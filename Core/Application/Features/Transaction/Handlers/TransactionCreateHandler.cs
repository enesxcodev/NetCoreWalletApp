using Application.Common;
using Application.Common.Constant;
using Application.Common.Enums;
using Application.Contracts;
using Application.Contracts.Events;
using Application.Features.Transaction.Commands;
using Domain.Common;
using Domain.Entities;
using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using WalletTransfer = Domain.Entities.Transaction;

namespace Application.Features.Transaction.Handlers;

public class TransactionCreateHandler(
    ITransaction transactionRepository,
    IWalletRepository walletRepository,
    IIdempotencyRepository idempotencyRepository,
    IUserContext userContext,
    ILogger<TransactionCreateHandler> logger,
    IPublishEndpoint publishEndpoint
) : IRequestHandler<TransactionCreateCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(TransactionCreateCommand request, CancellationToken cancellationToken)
    {
        var senderId = userContext.UserId;
        var requestHash = ComputeRequestHash(senderId, request);
        await idempotencyRepository.AcquireLockAsync(senderId, request.IdempotencyKey, cancellationToken);
        var existingRequest = await idempotencyRepository.GetAsync(senderId, request.IdempotencyKey, cancellationToken);

        if (existingRequest is not null)
        {
            if (!string.Equals(existingRequest.RequestHash, requestHash, StringComparison.Ordinal))
                return Result<Guid>.Failure("Aynı Idempotency-Key farklı bir istek için kullanılamaz.", ResultStatus.Conflict);

            logger.LogInformation(
                "Idempotent transfer cevabı tekrar oynatıldı. UserId: {UserId}, TransactionId: {TransactionId}, IdempotencyKey: {IdempotencyKey}",
                senderId, existingRequest.TransactionId, request.IdempotencyKey);
            return Result<Guid>.Success(existingRequest.TransactionId);
        }

        var senderWallet = await walletRepository.GetByUserIdAsync(senderId, cancellationToken);
        if (senderWallet is null)
            return Result<Guid>.Failure(Messages.Wallet.WalletNotFound);

        var receiverWallet = await walletRepository.GetByCodeAsync(request.WalletCode, cancellationToken);
        if (receiverWallet is null)
            return Result<Guid>.Failure(Messages.Wallet.ReceiveWalletNotFound);

        senderWallet.Withdraw(request.Amount);
        receiverWallet.Deposit(request.Amount);

        var transfer = new WalletTransfer(senderWallet.Id, receiverWallet.Id, request.Amount,
            request.Description ?? "transfer", Domain.Common.TransferType.Out);
        await transactionRepository.AddAsync(transfer);

        var response = Result<Guid>.Success(transfer.Id);
        var now = DateTime.UtcNow;
        await idempotencyRepository.AddAsync(new IdempotencyRecord(
            senderId, request.IdempotencyKey, requestHash, transfer.Id, (int)ResultStatus.Ok,
            JsonSerializer.Serialize(response), now, now.AddHours(24)), cancellationToken);

        await publishEndpoint.Publish(new MoneyTransferredEvent(
            transfer.Id, senderWallet.Id, senderWallet.Code, receiverWallet.Id, receiverWallet.Code,
            request.Amount, request.Description ?? "transfer", request.IdempotencyKey), cancellationToken);

        logger.LogInformation(
            "Transfer SQL transaction'ına ve outbox'a eklendi. UserId: {UserId}, TransactionId: {TransactionId}, IdempotencyKey: {IdempotencyKey}",
            senderId, transfer.Id, request.IdempotencyKey);
        return response;
    }

    private static string ComputeRequestHash(Guid userId, TransactionCreateCommand request)
    {
        var canonicalValue = string.Join('|', userId.ToString("N"), request.WalletCode.Trim().ToUpperInvariant(),
            request.Amount.ToString("0.############################", CultureInfo.InvariantCulture),
            (request.Description ?? "transfer").Trim());
        return Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(canonicalValue)));
    }
}
