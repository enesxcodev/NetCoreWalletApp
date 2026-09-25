using Application.Common;
using MediatR;
using System.Text.Json.Serialization;

namespace Application.Features.Transaction.Commands
{
    public record TransactionCreateCommand(string WalletCode, decimal Amount, string? Description = "transfer") : IRequest<Result<Guid>>, ITransactionalRequest
    {
        [JsonIgnore]
        public string IdempotencyKey { get; init; } = string.Empty;
    }
}
