using Application.Features.Transaction.Commands;
using Application.Features.Transaction.Queries;
using Application.Features.Wallet.Commands;
using Application.Features.Wallet.Queries;
using IMediator = MediatR.IMediator;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Application.Common;
using Application.Common.Enums;

namespace WebApi.Controllers
{
    [Authorize]
    public class WalletController(IMediator mediator) : CustomBaseController
    {
        [HttpGet]
        public async Task<IActionResult> Get(CancellationToken cancellationToken)
        {
            var result = await mediator.Send(new GetWalletSummaryQuery(), cancellationToken);
            return CreateActionResult(result);
        }

        [HttpGet("contacts")]
        public async Task<IActionResult> Contacts([FromQuery] SearchTransferContactsQuery query, CancellationToken cancellationToken)
        {
            var result = await mediator.Send(query, cancellationToken);
            return CreateActionResult(result);
        }

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
        public async Task<IActionResult> Transfer(
            [FromBody] TransactionCreateCommand command,
            [FromHeader(Name = "Idempotency-Key")] string? idempotencyKey)
        {
            if (!Guid.TryParse(idempotencyKey, out var parsedKey))
                return CreateActionResult(Result<Guid>.Failure(
                    "Idempotency-Key header'ı zorunludur ve geçerli bir UUID olmalıdır.",
                    ResultStatus.BadRequest));

            command = command with { IdempotencyKey = parsedKey.ToString("D") };
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
