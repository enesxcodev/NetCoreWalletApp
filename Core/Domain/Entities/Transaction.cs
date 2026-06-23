using Domain.Common;
using Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entities
{
    public class Transaction : BaseEntity
    {
        public Guid SenderWalletId { get; private set; }
        public Guid ReceiverWalletId { get; private set; }
        public decimal Amount { get; private set; }
        public string? Description { get; private set; }
        public TransferType Type { get; private set; }
        public Transaction()
        {

        }
        public Transaction(Guid senderId, Guid receiverId, decimal amount, string description = "transfer", TransferType type = TransferType.In)
        {
            CheckWalletId(senderId, receiverId);
            ValidateAmount(amount);
            SenderWalletId = senderId;
            ReceiverWalletId = receiverId;
            Amount = amount;
            Description = description;
            Type = type;
        }

        private void ValidateAmount(decimal Amount)
        {
            if (Amount <= 0) throw new DomainException("miktar 0 veya ondan küçük olamaz");
        }

        private void ValidateWalletId(Guid walletId)
        {
            if (walletId == Guid.Empty) throw new DomainException("WalletID boş olamaz");
        }
        private void CheckWalletId(Guid senderId, Guid reiceverId)
        {
            ValidateWalletId(senderId);
            ValidateWalletId(reiceverId);

            if (senderId == reiceverId)
                throw new DomainException("gönderen ve alıcı id aynı olamaz");
        }
    }
}
