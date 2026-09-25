using Domain.Common;

namespace Domain.Entities;

public class IdempotencyRecord : BaseEntity
{
    public Guid UserId { get; private set; }
    public string Key { get; private set; } = null!;
    public string RequestHash { get; private set; } = null!;
    public Guid TransactionId { get; private set; }
    public int HttpStatusCode { get; private set; }
    public string ResponseBody { get; private set; } = null!;
    public DateTime CreatedAtUtc { get; private set; }
    public DateTime ExpiresAtUtc { get; private set; }

    private IdempotencyRecord() { }

    public IdempotencyRecord(
        Guid userId,
        string key,
        string requestHash,
        Guid transactionId,
        int httpStatusCode,
        string responseBody,
        DateTime createdAtUtc,
        DateTime expiresAtUtc)
    {
        UserId = userId;
        Key = key;
        RequestHash = requestHash;
        TransactionId = transactionId;
        HttpStatusCode = httpStatusCode;
        ResponseBody = responseBody;
        CreatedAtUtc = createdAtUtc;
        ExpiresAtUtc = expiresAtUtc;
    }
}
