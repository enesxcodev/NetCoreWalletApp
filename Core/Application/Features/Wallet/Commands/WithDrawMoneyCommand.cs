using Application.Common;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Features.Wallet.Commands
{
    public record WithDrawMoneyCommand(decimal Amount) : IRequest<Result>;


}
