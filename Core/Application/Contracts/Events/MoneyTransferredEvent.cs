using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Contracts.Events
{
    public record MoneyTransferredEvent(
     Guid TransactionId,
     Guid SenderWalletId,
     string SenderWalletCode,
     Guid ReceiverWalletId,
     string ReceiverWalletCode,
     decimal Amount,
     string Description
 );
}
