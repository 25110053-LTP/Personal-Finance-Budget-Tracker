using BudgetTracker.Models;
using BudgetTracker.Recurrence;
using Xunit;

namespace BudgetTracker.Tests;

public class EndToEndTests
{
    [Fact] // HP-01: tạo tài khoản -> định kỳ -> thu/chi -> ngân sách -> báo cáo -> trend -> net worth
    public void FullFlow_Recurring_Budget_Report_Trend_NetWorth()
    {
        // 1. Tạo hai tài khoản trong Ledger
        var ledger = new Ledger();
        var checking = new Account("Ví chính", 5_000_000m);
        var card = new CreditCardAccount("Visa", 10_000_000m);
        ledger.AddAccount(checking);
        ledger.AddAccount(card);

        // 2. Giao dịch định kỳ tháng 03/2026
        var start = new DateOnly(2026, 3, 1);
        ledger.AddRule(new RecurringRule("RENT", "Ví chính", RecurringKind.Expense, 3_500_000m, "Tiền nhà", new MonthlySchedule(1), start));
        ledger.AddRule(new RecurringRule("SALARY", "Ví chính", RecurringKind.Income, 15_000_000m, "Lương", new MonthlySchedule(5), start));

        var created = ledger.RunRecurring(new DateOnly(2026, 3, 31));

        Assert.Equal(2, created.Count);
        Assert.Equal(16_500_000m, checking.Balance);               // 5tr - 3,5tr + 15tr
        Assert.Empty(ledger.RunRecurring(new DateOnly(2026, 3, 31))); // chạy lại không sinh trùng

        // 3. Chi tiêu thường ngày trên cả ví và thẻ
        checking.Record(new Expense(500_000m, "Ăn uống", new DateTime(2026, 3, 10)));
        card.Record(new Expense(400_000m, "Ăn uống", new DateTime(2026, 3, 12)));
        card.Record(new Expense(2_000_000m, "Mua sắm", new DateTime(2026, 3, 15)));

        List<ITransaction> All() => ledger.Accounts.SelectMany(a => a.Transactions).ToList();

        // 4. Ngân sách Ăn uống 1tr: đã chi 900k -> Warning, chi thêm 200k -> Exceeded
        var food = new Budget("Ăn uống", new Period(2026, 3), 1_000_000m);
        Assert.Equal(900_000m, food.Spent(All()));
        Assert.Equal(BudgetStatus.Warning, food.Evaluate(All()));

        checking.Record(new Expense(200_000m, "Ăn uống", new DateTime(2026, 3, 20)));
        Assert.Equal(BudgetStatus.Exceeded, food.Evaluate(All()));

        // 5. Báo cáo chi theo danh mục tháng 03/2026
        var report = new SpendingReportService();
        var rows = report.ByCategory(All(), new Period(2026, 3));

        Assert.Equal(3, rows.Count);
        Assert.Equal("Tiền nhà", rows[0].Category);
        Assert.Equal(3_500_000m, rows[0].Total);
        Assert.Equal("Mua sắm", rows[1].Category);
        Assert.Equal(2_000_000m, rows[1].Total);
        Assert.Equal("Ăn uống", rows[2].Category);
        Assert.Equal(1_100_000m, rows[2].Total);
        Assert.Equal(6_600_000m, report.TotalSpent(All(), new Period(2026, 3)));

        // 6. Trend: tháng 02/2026 không có chi tiêu -> không tính được phần trăm
        var trend = new TrendAnalyzer(report).CompareWithPreviousMonth(All(), new Period(2026, 3));
        Assert.Equal(0m, trend.Previous);
        Assert.Equal(6_600_000m, trend.Current);
        Assert.Null(trend.PercentChange);
        Assert.Equal(TrendDirection.Up, trend.Direction);

        // 7. Net worth: ví 15,8tr, nợ thẻ 2,4tr
        var nw = new NetWorthSummary(ledger.Accounts);
        Assert.Equal(15_800_000m, nw.TotalAssets);
        Assert.Equal(2_400_000m, nw.TotalLiabilities);
        Assert.Equal(13_400_000m, nw.NetWorth);
        Assert.Equal(nw.NetWorth, ledger.TotalBalance);
    }
}
