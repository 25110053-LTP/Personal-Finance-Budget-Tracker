namespace BudgetTracker.Models;

public enum TrendDirection { Up, Down, Flat }

public sealed record TrendResult(
    decimal Previous,
    decimal Current,
    decimal Change,
    decimal? PercentChange,
    TrendDirection Direction);

public class TrendAnalyzer
{
    private readonly SpendingReportService _report;
 
    public TrendAnalyzer(SpendingReportService report)
    {
        ArgumentNullException.ThrowIfNull(report);
        _report = report;
    }
 
    public TrendResult Compare(decimal previous, decimal current)
    {
        decimal change = current - previous;
        decimal? percent = previous == 0m ? null : Math.Round(change / previous * 100m, 1);
        var direction = change > 0 ? TrendDirection.Up
                      : change < 0 ? TrendDirection.Down
                      : TrendDirection.Flat;
        return new TrendResult(previous, current, change, percent, direction);
    }
    
    public TrendResult CompareWithPreviousMonth(IEnumerable<ITransaction> transactions, Period period)
    {
        ArgumentNullException.ThrowIfNull(transactions);
 
        var previousPeriod = Period.Of(new DateTime(period.Year, period.Month, 1).AddMonths(-1));
        return Compare(
            _report.TotalSpent(transactions, previousPeriod),
            _report.TotalSpent(transactions, period));
    }
}