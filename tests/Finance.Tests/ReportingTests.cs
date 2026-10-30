using BudgetTracker.Models;
using Xunit;

namespace BudgetTracker.Tests;

public class ReportingTests
{
    private readonly SpendingReportService _report = new();
    private readonly TrendAnalyzer _trend;

    public ReportingTests() => _trend = new TrendAnalyzer(_report);

    private static Expense Exp(decimal amount, string category, int y, int m, int d) =>
        new(amount, category, new DateTime(y, m, d));

    private static readonly Period Oct2026 = new(2026, 10);

    [Fact] // RP-01: gộp theo category, tính %, sắp giảm dần
    public void ByCategory_GroupsPercentAndSortsDescending()
    {
        var txs = new List<ITransaction>
        {
            Exp(100_000m, "Ăn uống", 2026, 10, 3),
            Exp(50_000m,  "Ăn uống", 2026, 10, 5),
            Exp(300_000m, "Tiền nhà", 2026, 10, 7),
        };

        var r = _report.ByCategory(txs, Oct2026);

        Assert.Equal(2, r.Count);
        Assert.Equal("Tiền nhà", r[0].Category);
        Assert.Equal(66.7m, r[0].Percent);
        Assert.Equal(150_000m, r[1].Total);
        Assert.Equal(33.3m, r[1].Percent);
    }

    [Fact] // RP-02: bỏ qua giao dịch ngoài kỳ và Income
    public void ByCategory_IgnoresOtherPeriodsAndIncome()
    {
        var txs = new List<ITransaction>
        {
            Exp(100_000m, "Ăn uống", 2026, 9, 30),
            new Income(5_000_000m, "Lương", new DateTime(2026, 10, 1)),
        };

        Assert.Empty(_report.ByCategory(txs, Oct2026));
        Assert.Equal(0m, _report.TotalSpent(txs, Oct2026));
    }

    [Fact] // RP-03: tháng trước bằng 0 thì % là null
    public void Trend_PreviousZero_PercentIsNull()
    {
        var t = _trend.Compare(0m, 200_000m);

        Assert.Null(t.PercentChange);
        Assert.Equal(TrendDirection.Up, t.Direction);
    }

    [Fact] // RP-04: tháng 1 so với tháng 12 năm trước
    public void Trend_JanuaryComparesWithPreviousDecember()
    {
        var txs = new List<ITransaction>
        {
            Exp(100_000m, "Ăn uống", 2025, 12, 10),
            Exp(150_000m, "Ăn uống", 2026, 1, 10),
        };

        var t = _trend.CompareWithPreviousMonth(txs, new Period(2026, 1));

        Assert.Equal(50.0m, t.PercentChange);
        Assert.Equal(TrendDirection.Up, t.Direction);
    }

    [Fact] // RP-05: chi giảm
    public void Trend_Decrease_IsDown()
    {
        var t = _trend.Compare(200_000m, 150_000m);

        Assert.Equal(-25.0m, t.PercentChange);
        Assert.Equal(TrendDirection.Down, t.Direction);
    }
}