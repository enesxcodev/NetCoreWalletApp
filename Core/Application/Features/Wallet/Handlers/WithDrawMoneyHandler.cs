using Application.Common;
using Application.Common.Constant;
using Application.Contracts;
using Application.Features.Wallet.Commands;
using MediatR;

namespace Application.Features.Wallet.Handlers
{
    public class WithDrawMoneyHandler(IWalletRepository walletRepository, IUserContext userContext, IUnitOfWork unitOfWork) : IRequestHandler<WithDrawMoneyCommand, Result>
    {
        public async Task<Result> Handle(WithDrawMoneyCommand request, CancellationToken cancellationToken)
        {
            var wallet = await walletRepository.GetByUserIdAsync(userContext.UserId, cancellationToken);
            if (wallet is null)
                Result.Failure(Messages.Wallet.WalletNotFound_TWO, Common.Enums.ResultStatus.NoContent);

            wallet.Withdraw(request.Amount);
            walletRepository.Update(wallet);
            await unitOfWork.SaveChangesAsync();
            return Result.Success();
        }
    }
}
