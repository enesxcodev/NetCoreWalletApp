using Application.Contracts;
using Domain.Entities;
using Persistence.Context;
using System;
using System.Collections.Generic;
using System.Text;

namespace Persistence.Repository
{
    public class TransactionRepository(AppDbContext context) : GenericRepository<Transaction>(context), ITransaction
    {
    }
}
