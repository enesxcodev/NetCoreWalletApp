using Application.Common;
using Application.Common.Constant;
using Application.Contracts;
using Application.Features.Transaction.Queries;
using Application.Features.Transaction.Results;
using Dapper;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using System.Data;

namespace Application.Features.Transaction.Handlers
{
    public class TransactionHistoryHandlerDapper(
        IDbConnection db,
        IUserContext userContext,
        ITransaction transactionRepository,
        IWalletRepository walletRepository,
        IDistributedCache cache
        )
    : IRequestHandler<GetTransactionHistoryQuery, Result<IEnumerable<TransactionHistoryResult>>>
    {
        public async Task<Result<IEnumerable<TransactionHistoryResult>>> Handle(GetTransactionHistoryQuery request, CancellationToken cancellationToken)
        {
            var currentUserId = userContext.UserId;
            var wallet = await walletRepository.GetByUserIdAsync(currentUserId, cancellationToken);
            if (wallet is null)
                return Result<IEnumerable<TransactionHistoryResult>>.Failure(Messages.Wallet.TransactionNotFoundWallet);

            string cacheKey = $"tx_history:{wallet.Id}:p_{request.PageNumber}:s_{request.PageSize}";
            var cachedData = await cache.GetAsync<IEnumerable<TransactionHistoryResult>>(cacheKey, cancellationToken);
            if (cachedData is not null)                            
                return Result<IEnumerable<TransactionHistoryResult>>.Success(cachedData);
            
            string sqlQuery = @"SELECT     
                CASE 
                    WHEN t.SenderWalletId = @WalletId THEN rW.Code
                    ELSE sW.Code
                END AS WalletCode,
                t.Amount,
                COALESCE(t.Description, 'transfer') AS Description,
                CASE 
                    WHEN t.SenderWalletId = @WalletId THEN 'Out'
                    ELSE 'In'
                END AS Type,
                t.CreatedAt
            FROM Transactions t
            INNER JOIN Wallets sW ON t.SenderWalletId = sW.Id   -- Gönderen cüzdanı 
            INNER JOIN Wallets rW ON t.ReceiverWalletId = rW.Id -- Alıcı cüzdanı
            WHERE t.SenderWalletId = @WalletId OR t.ReceiverWalletId = @WalletId
            ORDER BY t.CreatedAt DESC
            OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;
";

            var parameters = new
            {
                WalletId = wallet.Id,
                Offset = (request.PageNumber - 1) * request.PageSize,
                PageSize = request.PageSize
            };

            var resultList = await db.QueryAsync<TransactionHistoryResult>(sqlQuery, parameters);

            if (resultList.Any())
            {
                await cache.SetAsync(cacheKey, resultList, TimeSpan.FromMinutes(2), cancellationToken);
            }
            return Result<IEnumerable<TransactionHistoryResult>>.Success(resultList);
        }
    }
}
