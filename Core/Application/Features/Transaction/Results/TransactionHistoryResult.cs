using Application.Common.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Features.Transaction.Results
{
    public record TransactionHistoryResult(
        string WalletCode, 
        decimal Amount,
        string Description,
        string Type,
        DateTime CreatedAt
    );
}
