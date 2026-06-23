using Application.Features.Transaction.Commands;
using Application.Features.Transaction.Queries;
using Application.Features.Wallet.Commands;
using IMediator = MediatR.IMediator;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    //[Authorize]
    public class WalletController(IMediator mediator) : CustomBaseController
    {
        [HttpPost("deposit")]
        public async Task<IActionResult> Deposit([FromBody] DepositMoneyCommand command)
        {
            var result = await mediator.Send(command);
            return CreateActionResult(result);
        }

        [HttpPost("withdraw")]
        public async Task<IActionResult> Withdraw([FromBody] WithDrawMoneyCommand command)
        {
            var result = await mediator.Send(command);
            return CreateActionResult(result);
        }

        [HttpPost("transfer")]
        public async Task<IActionResult> Transfer([FromBody] TransactionCreateCommand command)
        {
            var result = await mediator.Send(command);
            return CreateActionResult(result);
        }

        [HttpGet("transaction")]
        public async Task<IActionResult> Transaction([FromQuery] GetTransactionHistoryQuery query)
        {
            var result = await mediator.Send(query);
            return CreateActionResult(result);
        }
    }
}
