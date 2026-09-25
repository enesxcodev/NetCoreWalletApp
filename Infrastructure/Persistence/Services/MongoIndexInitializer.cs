using Microsoft.Extensions.Hosting;
using MongoDB.Driver;
using Persistence.Models;

namespace Persistence.Services;

public class MongoIndexInitializer(IMongoDatabase database) : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var collection = database.GetCollection<TransactionDocument>("AuditTransactions");
        var index = new CreateIndexModel<TransactionDocument>(
            Builders<TransactionDocument>.IndexKeys.Ascending(x => x.TransactionId),
            new CreateIndexOptions { Unique = true, Name = "UX_AuditTransactions_TransactionId" });
        await collection.Indexes.CreateOneAsync(index, cancellationToken: cancellationToken);
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
