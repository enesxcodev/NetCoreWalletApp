using Application.Contracts;
using Application.Features.Wallet.Queries;
using Microsoft.EntityFrameworkCore;
using Persistence.Context;

namespace Persistence.Services;

/// <summary>Ad, kullanıcı adı veya e-postaya göre transfer alıcılarını SQL'de filtreler.</summary>
public class TransferContactReader(AppDbContext context) : ITransferContactReader
{
    public async Task<TransferContactsPage> SearchAsync(
        Guid currentUserId, string search, int page, int pageSize, CancellationToken cancellationToken)
    {
        var contacts = from user in context.AppUsers.AsNoTracking()
                       join wallet in context.Wallets.AsNoTracking() on user.Id equals wallet.UserId
                       where user.Id != currentUserId
                       select new { User = user, Wallet = wallet };

        if (!string.IsNullOrEmpty(search))
            contacts = contacts.Where(x =>
                x.User.FirstName.Contains(search) ||
                x.User.LastName.Contains(search) ||
                (x.User.FirstName + " " + x.User.LastName).Contains(search) ||
                x.User.UserName.Contains(search) ||
                x.User.Email.Contains(search));

        var totalCount = await contacts.CountAsync(cancellationToken);
        var items = await contacts
            .OrderBy(x => x.User.FirstName)
            .ThenBy(x => x.User.LastName)
            .ThenBy(x => x.User.UserName)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(x => new TransferContactResult(
                x.User.FirstName, x.User.LastName, x.User.UserName, x.Wallet.Code))
            .ToListAsync(cancellationToken);

        return new TransferContactsPage(items, totalCount, page, pageSize);
    }
}
