using BudgetTracker.Models;
using BudgetTracker.Recurrence;
using Xunit;

namespace BudgetTracker.Tests;

public class RecurringTests
{
    private static DateOnly D(int y, int m, int d) => new(y, m, d);
    [Fact(DisplayName = "RC-01 Monthly: ngày 15 mỗi tháng")]
    public void RC01_Monthly_FixedDay()
    {
        var s = new MonthlySchedule(15);
        var days = s.GetOccurrences(D(2025, 1, 1), D(2025, 3, 31)).ToList();
        Assert.Equal(new[] { D(2025, 1, 15), D(2025, 2, 15), D(2025, 3, 15) }, days);
    }
    [Fact(DisplayName = "RC-02 Monthly: ngày 31 lùi về cuối tháng (kể cả năm nhuận)")]
    public void RC02_Monthly_ClampToEndOfMonth()
    {
        var s = new MonthlySchedule(31);
        Assert.Equal(
            new[] { D(2025, 1, 31), D(2025, 2, 28), D(2025, 3, 31), D(2025, 4, 30) },
            s.GetOccurrences(D(2025, 1, 1), D(2025, 4, 30)));
        Assert.Contains(D(2024, 2, 29), s.GetOccurrences(D(2024, 2, 1), D(2024, 2, 29)));
    }
    [Fact(DisplayName = "RC-03 Weekly: mỗi thứ Hai")]
    public void RC03_Weekly()
    {
        var s = new WeeklySchedule(DayOfWeek.Monday);
        var days = s.GetOccurrences(D(2025, 1, 1), D(2025, 1, 31)).ToList(); // 1/1/2025 là thứ Tư
        Assert.Equal(new[] { D(2025, 1, 6), D(2025, 1, 13), D(2025, 1, 20), D(2025, 1, 27) }, days);
        Assert.Equal(D(2025, 1, 6), s.NextOccurrence(D(2025, 1, 1)));
        Assert.Equal(D(2025, 1, 13), s.NextOccurrence(D(2025, 1, 6)));
    }
    [Fact(DisplayName = "RC-04 RunRecurring idempotent: chạy lại không ghi trùng")]
    public void RC04_RunRecurring_Idempotent()
    {
        var ledger = new Ledger();
        ledger.AddAccount(new Account("CHK", 5000m));
        ledger.AddRule(new RecurringRule("RENT", "CHK", RecurringKind.Expense, 1000m, "Nhà",
            new MonthlySchedule(1), D(2025, 1, 1)));

        Assert.Equal(3, ledger.RunRecurring(D(2025, 3, 31)).Count);
        Assert.Empty(ledger.RunRecurring(D(2025, 3, 31)));
        Assert.Single(ledger.RunRecurring(D(2025, 4, 30)));
        Assert.Equal(5000m - 4 * 1000m, ledger.GetAccount("CHK").Balance);
    }

    [Fact(DisplayName = "RC-05 Generator tôn trọng EndDate và IsActive")]
    public void RC05_Generator_EndDate_And_Inactive()
    {
        var gen = new RecurringGenerator();

        var limited = new RecurringRule("R1", "CHK", RecurringKind.Expense, 50m, "Gói",
            new MonthlySchedule(10), D(2025, 1, 1), endDate: D(2025, 2, 28));
        Assert.Equal(2, gen.GetDueDates(limited, D(2025, 12, 31)).Count);

        var paused = new RecurringRule("R2", "CHK", RecurringKind.Expense, 50m, "Tạm dừng",
            new MonthlySchedule(10), D(2025, 1, 1)) { IsActive = false };
        Assert.Empty(gen.GetDueDates(paused, D(2025, 12, 31)));
    }
    [Fact(DisplayName = "RC-06 Ledger xử lý theo thứ tự thời gian giữa các rule (thu trước chi)")]
    public void RC06_Ledger_ChronologicalOrder()
    {
        var ledger = new Ledger();
        ledger.AddAccount(new Account("CHK", 0m));
        ledger.AddRule(new RecurringRule("RENT", "CHK", RecurringKind.Expense, 800m, "Nhà", new MonthlySchedule(20), D(2025, 1, 1)));
        ledger.AddRule(new RecurringRule("PAY", "CHK", RecurringKind.Income, 1000m, "Lương", new MonthlySchedule(10), D(2025, 1, 1)));
        ledger.RunRecurring(D(2025, 1, 31));
        Assert.Equal(200m, ledger.GetAccount("CHK").Balance);
    }
    [Fact(DisplayName = "RC-07 Không đủ số dư: ném lỗi và rule không bị đánh dấu đã sinh")]
    public void RC07_InsufficientFunds_DoesNotAdvanceRule()
    {
        var ledger = new Ledger();
        ledger.AddAccount(new Account("CHK", 100m));
        var rule = new RecurringRule("RENT", "CHK", RecurringKind.Expense, 500m, "Nhà", new MonthlySchedule(5), D(2025, 1, 1));
        ledger.AddRule(rule);

        Assert.Throws<InsufficientFundsException>(() => ledger.RunRecurring(D(2025, 1, 31)));
        Assert.Null(rule.LastGeneratedDate);
        Assert.Equal(100m, ledger.GetAccount("CHK").Balance);
    }

    [Fact(DisplayName = "PL-03 Ledger: nhiều Account, rule chạy đúng account của nó")]
    public void PL03_Ledger_MultipleAccounts()
    {
        var ledger = new Ledger();
        ledger.AddAccount(new Account("CHK", 5000m));
        ledger.AddAccount(new Account("SAV", 10000m));

        ledger.AddRule(new RecurringRule("RENT", "CHK", RecurringKind.Expense, 1000m, "Nhà",
            new MonthlySchedule(1), D(2025, 1, 1)));
        ledger.AddRule(new RecurringRule("SAVE", "SAV", RecurringKind.Income, 200m, "Tiết kiệm",
            new WeeklySchedule(DayOfWeek.Friday), D(2025, 1, 1)));

        ledger.RunRecurring(D(2025, 1, 31));

        Assert.Equal(5000m - 1000m, ledger.GetAccount("CHK").Balance);       // 1 lần (1/1)
        Assert.Equal(10000m + 5 * 200m, ledger.GetAccount("SAV").Balance);   // 5 thứ Sáu: 3,10,17,24,31
        Assert.Equal(4000m + 11000m, ledger.TotalBalance);
        Assert.Throws<KeyNotFoundException>(() => ledger.GetAccount("XXX"));
        Assert.Throws<KeyNotFoundException>(() => ledger.AddRule(new RecurringRule("BAD", "XXX",
            RecurringKind.Income, 1m, "x", new MonthlySchedule(1), D(2025, 1, 1))));
        Assert.Throws<InvalidOperationException>(() => ledger.AddAccount(new Account("chk")));
    }
}