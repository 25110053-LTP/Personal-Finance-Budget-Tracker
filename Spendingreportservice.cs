using System.Data;
using System.Transactions;

namespace BudgetTracker.Models;

public sealed record CategorySpending(string Category, decimal Total, decimal Percent);

public class SpendingReportService
{
    public IReadOnlyList<CategorySpending> ByCategory(IEnumerable<ITransaction> transactions, DateOnly from, DateOnly to)
    {
        
    }
}