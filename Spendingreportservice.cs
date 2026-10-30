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

        return expenses.GroupBy(e => e.Category).Select(g =>
            {
                decimal total = g.Sum(e => e.Amount);
                decimal percent = grand == 0 ? 0m : Math.Round(total / grand * 100m, 1);
                return new CategorySpending(g.Key.Name, total, percent);
            })
            .OrderByDescending(c => c.Total).ThenBy(c => c.Category, StringComparer.CurrentCulture).ToList();
    }

    public decimal TotalSpent(IEnumerable<ITransaction> transactions, Period period)
    {
        ArgumentNullException.ThrowIfNull(transactions);
 
        return transactions
            .OfType<Expense>()
            .Where(e => period.Contains(e.Date))
            .Sum(e => e.Amount);
    }
}