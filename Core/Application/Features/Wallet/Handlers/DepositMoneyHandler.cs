using Application.Common;
using Application.Common.Constant;
using Application.Contracts;
using Application.Features.Wallet.Commands;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Features.Wallet.Handlers
{
    public class DepositMoneyHandler(IWalletRepository walletRepository, IUserContext userContext, IUnitOfWork unitOfWork, ILogger<DepositMoneyHandler> logger) : IRequestHandler<DepositMoneyCommand, Result>
    {
        public async Task<Result> Handle(DepositMoneyCommand request, CancellationToken cancellationToken)
        {
            var wallet = await walletRepository.GetByUserIdAsync(userContext.UserId, cancellationToken);

            if (wallet is null)
                return Result.Failure(Messages.Wallet.WalletNotFound_TWO);

            wallet.Deposit(request.Amount);
            walletRepository.Update(wallet);
            await unitOfWork.SaveChangesAsync();

            logger.LogInformation($"{userContext.UserId} nolu kullanıcı {request.Amount} bakiye ekledi");

            return Result.Success();
        }

    }
}
