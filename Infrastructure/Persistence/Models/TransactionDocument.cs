using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace Persistence.Models
{
    public class TransactionDocument
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)] // 🎯 C#'ta Guid, Mongo'da string (UUID) olacak
        public Guid Id { get; set; }

        [BsonRepresentation(BsonType.String)]
        public Guid TransactionId { get; set; }

        [BsonRepresentation(BsonType.String)]
        public Guid SenderWalletId { get; set; }

        public string SenderWalletCode { get; set; } = null!;

        [BsonRepresentation(BsonType.String)]
        public Guid ReceiverWalletId { get; set; }

        public string ReceiverWalletCode { get; set; } = null!;

        public decimal Amount { get; set; }

        public string Description { get; set; } = null!;

        public string Type { get; set; } = null!;

        public DateTime CreatedAt { get; set; }
    }
}