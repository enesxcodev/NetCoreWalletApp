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
    public class TransactionHistoryHandler(
        IDbConnection db,
        IUserContext userContext,
        ITransaction transactionRepository,
        IWalletRepository walletRepository,
        IDistributedCache cache,
        ITransactionAuditService auditService
        ): IRequestHandler<GetTransactionHistoryQuery, Result<IEnumerable<TransactionHistoryResult>>>
    {
        public async Task<Result<IEnumerable<TransactionHistoryResult>>> Handle(GetTransactionHistoryQuery request, CancellationToken cancellationToken)
        {
            var currentUserId = userContext.UserId;
            var wallet = await walletRepository.GetByUserIdAsync(currentUserId, cancellationToken);
            if (wallet is null)
                return Result<IEnumerable<TransactionHistoryResult>>.Failure(Messages.Wallet.TransactionNotFoundWallet);

            string cacheKey = $"tx_history:{wallet.Id}:p_{request.PageNumber}:s_{request.PageSize}";

            // 1. Önce Redis'e bakıyoruz 
            var cachedData = await cache.GetAsync<IEnumerable<TransactionHistoryResult>>(cacheKey, cancellationToken);
            if (cachedData is not null)
                return Result<IEnumerable<TransactionHistoryResult>>.Success(cachedData);

            // 2. 🎯 REDIS'TE YOKSA: Artık SQL yerine MongoDB'den ışık hızıyla çekiyoruz!
            var resultList = await auditService.GetHistoryAsync(
                wallet.Id,
                request.PageNumber,
                request.PageSize,
                cancellationToken
            );

            // 3. Çekilen veriyi yine sonraki seferler için Redis'e yazıyoruz
            if (resultList.Any())
            {
                await cache.SetAsync(cacheKey, resultList, TimeSpan.FromMinutes(2), cancellationToken);
            }

            return Result<IEnumerable<TransactionHistoryResult>>.Success(resultList);
        }
    }
}
