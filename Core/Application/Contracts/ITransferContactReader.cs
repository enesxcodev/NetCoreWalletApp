using Application.Features.Wallet.Queries;

namespace Application.Contracts;

/// <summary>Transfer alıcılarını kullanıcı ve cüzdan kayıtlarından okur.</summary>
public interface ITransferContactReader
{
    Task<TransferContactsPage> SearchAsync(
        Guid currentUserId, string search, int page, int pageSize, CancellationToken cancellationToken);
}
