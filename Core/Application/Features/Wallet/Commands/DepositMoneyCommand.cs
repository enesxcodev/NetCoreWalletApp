using Application.Common;
using MediatR;

namespace Application.Features.Wallet.Commands
{
    public record DepositMoneyCommand(decimal Amount) : IRequest<Result>;
}
