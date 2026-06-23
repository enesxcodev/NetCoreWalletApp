using Application.Contracts;
using Application.Features.Transaction.Results;
using MongoDB.Driver;
using Persistence.Models;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Persistence.Services;

public class TransactionAuditService(IMongoDatabase mongoDb) : ITransactionAuditService
{
    public async Task IdempotentLogAsync(Guid transactionId, Guid senderId, string senderCode, Guid receiverId, string receiverCode, decimal amount, string description, string type, CancellationToken cancellationToken)
    {
        var collection = mongoDb.GetCollection<TransactionDocument>("AuditTransactions");

        var document = new TransactionDocument
        {            
            TransactionId = transactionId,
            SenderWalletId = senderId,
            SenderWalletCode = senderCode,
            ReceiverWalletId = receiverId,
            ReceiverWalletCode = receiverCode,
            Amount = amount,
            Description = description,
            Type = type,
            CreatedAt = DateTime.UtcNow
        };

        await collection.InsertOneAsync(document, null, cancellationToken);
    }

    public async Task<IEnumerable<TransactionHistoryResult>> GetHistoryAsync(Guid walletId, int pageNumber, int pageSize, CancellationToken cancellationToken)
    {
        var collection = mongoDb.GetCollection<TransactionDocument>("AuditTransactions");

        // 🎯 MongoDB Filtresi: Gönderen VEYA Alıcı cüzdanı bizim cüzdan mı?
        var filter = Builders<TransactionDocument>.Filter.Or(
            Builders<TransactionDocument>.Filter.Eq(x => x.SenderWalletId, walletId),
            Builders<TransactionDocument>.Filter.Eq(x => x.ReceiverWalletId, walletId)
        );

        // Sayfalama ve sıralama işlemini yapıp veriyi çekiyoruz 
        var documents = await collection.Find(filter)
            .SortByDescending(x => x.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Limit(pageSize)
            .ToListAsync(cancellationToken);

        // 🎯 Mongo dökümanını direkt senin 'TransactionHistoryResult' DTO'suna mapliyoruz
        return documents.Select(doc => new TransactionHistoryResult(
                 doc.SenderWalletId == walletId ? doc.ReceiverWalletCode : doc.SenderWalletCode,
                 doc.Amount,
                 doc.Description,
                 doc.SenderWalletId == walletId ? "Out" : "In",
                 doc.CreatedAt
             ));
    }
}