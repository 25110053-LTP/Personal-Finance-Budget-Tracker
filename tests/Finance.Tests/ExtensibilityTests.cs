using BudgetTracker.Models;
using BudgetTracker.Recurrence;
using Xunit;

namespace BudgetTracker.Tests;

public class ExtensibilityTests
{
    // Một kiểu lịch MỚI, viết ngay trong test: không phải sửa MonthlySchedule, RecurringRule, RecurringGenerator hay Ledger.
    private sealed class EveryNDaysSchedule : IRecurrenceSchedule
    {
        private readonly int _n;
        public EveryNDaysSchedule(int n) => _n = n;

        public IEnumerable<DateOnly> GetOccurrences(DateOnly from, DateOnly to)
        {
            for (var d = from; d <= to; d = d.AddDays(_n))
                yield return d;
        }

        public DateOnly NextOccurrence(DateOnly after) => after.AddDays(_n);
    }

    [Fact] // PL-02: cùng một khoản chi, tài khoản khác loại thì hành vi khác (Account.Withdraw là virtual)
    public void SameExpense_BehavesDifferently_ByAccountType()
    {
        ITransaction purchase = new Expense(5_000_000m, "Mua sắm");
        Account checking = new Account("Ví", 1_000_000m);
        Account creditCard = new CreditCardAccount("Visa", 10_000_000m);

        Assert.Throws<InsufficientFundsException>(() => checking.Record(purchase));
        Assert.Equal(1_000_000m, checking.Balance);

        creditCard.Record(purchase);
        Assert.Equal(-5_000_000m, creditCard.Balance);
    }

    [Fact] // PL-03a: đổi IRecurrenceSchedule thì số lần sinh giao dịch đổi theo, RecurringGenerator không đổi
    public void SwappingSchedule_ChangesGeneratedDates()
    {
        var start = new DateOnly(2026, 3, 1);
        var upTo = new DateOnly(2026, 3, 31);
        var generator = new RecurringGenerator();

        RecurringRule Rule(IRecurrenceSchedule s) =>
            new("R", "Ví", RecurringKind.Expense, 100_000m, "Ăn uống", s, start);

        Assert.Single(generator.GetDueDates(Rule(new MonthlySchedule(1)), upTo));                      // 01/03
        Assert.Equal(5, generator.GetDueDates(Rule(new WeeklySchedule(DayOfWeek.Monday)), upTo).Count); // 2, 9, 16, 23, 30
        Assert.Equal(4, generator.GetDueDates(Rule(new EveryNDaysSchedule(10)), upTo).Count);           // 1, 11, 21, 31
    }

    [Fact] // PL-03b: lịch mới chạy được qua Ledger, vẫn không sinh trùng khi chạy lại
    public void CustomSchedule_WorksThroughLedger()
    {
        var ledger = new Ledger();
        var account = ledger.AddAccount(new Account("Ví", 1_000_000m));
        ledger.AddRule(new RecurringRule("GYM", "Ví", RecurringKind.Expense, 100_000m,
            "Thể thao", new EveryNDaysSchedule(10), new DateOnly(2026, 3, 1)));

        var created = ledger.RunRecurring(new DateOnly(2026, 3, 31));

        Assert.Equal(4, created.Count);
        Assert.Equal(600_000m, account.Balance);
        Assert.Empty(ledger.RunRecurring(new DateOnly(2026, 3, 31)));
    }
}
