using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Contracts
{
    public interface IWalletRepository : IRepository<Wallet>
    {
        Task<Wallet?> GetByUserIdAsync(Guid userId, CancellationToken token);
        Task<Wallet?> GetByCodeAsync(string walletCode, CancellationToken token);
    }
}
