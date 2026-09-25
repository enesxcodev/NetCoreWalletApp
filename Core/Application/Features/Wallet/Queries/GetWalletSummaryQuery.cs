using Application.Common;
using Application.Contracts;
using MediatR;

namespace Application.Features.Wallet.Queries;

public record WalletSummaryResult(string Code, decimal Balance);

public record GetWalletSummaryQuery : IRequest<Result<WalletSummaryResult>>;

/// <summary>Oturumdaki kullanıcının güncel cüzdan kodunu ve bakiyesini okur.</summary>
public class GetWalletSummaryHandler(IWalletRepository wallets, IUserContext userContext)
    : IRequestHandler<GetWalletSummaryQuery, Result<WalletSummaryResult>>
{
    public async Task<Result<WalletSummaryResult>> Handle(GetWalletSummaryQuery request, CancellationToken cancellationToken)
    {
        var wallet = await wallets.GetByUserIdAsync(userContext.UserId, cancellationToken);
        return wallet is null
            ? Result<WalletSummaryResult>.Failure("Cüzdan bulunamadı", Application.Common.Enums.ResultStatus.NotFound)
            : Result<WalletSummaryResult>.Success(new WalletSummaryResult(wallet.Code, wallet.Balance));
    }
}
