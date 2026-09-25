using Application.Common;
using Application.Contracts;
using MediatR;

namespace Application.Features.Wallet.Queries;

public record TransferContactResult(string FirstName, string LastName, string UserName, string WalletCode);

public record TransferContactsPage(IReadOnlyList<TransferContactResult> Items, int TotalCount, int Page, int PageSize);

public record SearchTransferContactsQuery(string? Search = null, int Page = 1, int PageSize = 10)
    : IRequest<Result<TransferContactsPage>>;

/// <summary>Arama terimini ve sayfa sınırlarını düzenleyip alıcı listesini okur.</summary>
public class SearchTransferContactsHandler(ITransferContactReader reader, IUserContext userContext)
    : IRequestHandler<SearchTransferContactsQuery, Result<TransferContactsPage>>
{
    public async Task<Result<TransferContactsPage>> Handle(
        SearchTransferContactsQuery request, CancellationToken cancellationToken)
    {
        if (request.Page < 1 || request.PageSize is < 1 or > 50)
            return Result<TransferContactsPage>.Failure("Geçerli bir sayfa ve 1–50 arasında kayıt sayısı girin.");

        var search = request.Search?.Trim() ?? string.Empty;
        if (search.Length > 100)
            return Result<TransferContactsPage>.Failure("Arama metni en fazla 100 karakter olabilir.");

        var result = await reader.SearchAsync(userContext.UserId, search, request.Page, request.PageSize, cancellationToken);
        return Result<TransferContactsPage>.Success(result);
    }
}
