using System.Data;
using System.Transactions;

namespace BudgetTracker.Models;

public sealed record CategorySpending(string Category, decimal Total, decimal Percent);

public class SpendingReportService
{
    public IReadOnlyList<CategorySpending> ByCategory(IEnumerable<ITransaction> transactions, Period period)
    {
        ArgumentNullException. ThrowIfNull(transactions);

        var expenses = transactions.OfType<Expense>().Where(e => period.Contains(e.Date)).ToList();

        decimal grand = expenses.Sum(e => e.Amount);


    }
}