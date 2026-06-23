using Application.Common;
using Application.Features.Transaction.Results;
using MediatR;

namespace Application.Features.Transaction.Queries
{
    public record GetTransactionHistoryQuery(int PageNumber = 1, int PageSize = 10) : IRequest<Result<IEnumerable<TransactionHistoryResult>>>;
}
