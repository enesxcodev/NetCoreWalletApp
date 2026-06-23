using Application.Contracts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Persistence.Context
{
    public class UnitOfWork(AppDbContext context) : IUnitOfWork
    {
        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await context.SaveChangesAsync(cancellationToken);
        }

        public void Dispose()
        {
            context.Dispose();
            GC.SuppressFinalize(this);
        }

        public async Task BeginTransactionAsync(CancellationToken cancellationToken = default) => await context.Database.BeginTransactionAsync(cancellationToken);
        public async Task CommitTransactionAsync(CancellationToken cancellationToken = default) => await context.Database.CommitTransactionAsync(cancellationToken);
        public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default) => await context.Database.RollbackTransactionAsync(cancellationToken);
    }
}
