using Application.Features.Transaction.Results;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Contracts
{
    public interface ITransactionAuditService
    {
        //mongodb için
        Task IdempotentLogAsync(Guid transactionId, Guid senderId, string senderCode, Guid receiverId, string receiverCode, decimal amount, string description, string type, CancellationToken cancellationToken);

        Task<IEnumerable<TransactionHistoryResult>> GetHistoryAsync(Guid walletId, int pageNumber, int pageSize, CancellationToken cancellationToken);
    }
}
