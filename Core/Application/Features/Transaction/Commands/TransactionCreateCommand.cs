using Application.Common;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Features.Transaction.Commands
{
    public record TransactionCreateCommand(string WalletCode, decimal Amount, string? Description = "transfer") : IRequest<Result>, ITransactionalRequest;
}
