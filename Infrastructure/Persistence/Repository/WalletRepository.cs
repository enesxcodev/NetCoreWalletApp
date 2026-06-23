using Application.Contracts;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Persistence.Context;
using System;
using System.Collections.Generic;
using System.Text;

namespace Persistence.Repository
{
    public class WalletRepository(AppDbContext context) : GenericRepository<Wallet>(context), IWalletRepository
    {
        public async Task<Wallet?> GetByCodeAsync(string walletCode, CancellationToken token)
        {
            return await context.Wallets.FirstOrDefaultAsync(x => x.Code == walletCode, token);
        }

        public async Task<Wallet?> GetByUserIdAsync(Guid userId, CancellationToken token)
        {
            return await context.Wallets.FirstOrDefaultAsync(x => x.UserId == userId, token);
        }
    }
}
