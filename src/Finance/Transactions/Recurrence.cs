using BudgetTracker.Models;
namespace BudgetTracker.Recurrence;

public interface IRecurrenceSchedule
{
    IEnumerable<DateOnly> GetOccurrences(DateOnly from, DateOnly to);
    DateOnly NextOccurrence(DateOnly after);
}

public sealed class MonthlySchedule : IRecurrenceSchedule
{
    public int DayOfMonth { get; }
    public MonthlySchedule(int dayOfMonth)
    {
        if (dayOfMonth is < 1 or > 31)
            throw new ArgumentOutOfRangeException(nameof(dayOfMonth), "Ngày phải trong khoảng 1..31.");
        DayOfMonth = dayOfMonth;
    }
    private DateOnly InMonth(int year, int month) =>
        new(year, month, Math.Min(DayOfMonth, DateTime.DaysInMonth(year, month)));
    public IEnumerable<DateOnly> GetOccurrences(DateOnly from, DateOnly to)
    {
        if (to < from) yield break;
        var cursor = new DateOnly(from.Year, from.Month, 1);
        while (cursor <= to)
        {
            var d = InMonth(cursor.Year, cursor.Month);
            if (d >= from && d <= to) yield return d;
            cursor = cursor.AddMonths(1);
        }
    }
    public DateOnly NextOccurrence(DateOnly after)
    {
        var cursor = new DateOnly(after.Year, after.Month, 1);
        var d = InMonth(cursor.Year, cursor.Month);
        if (d > after) return d;
        cursor = cursor.AddMonths(1);
        return InMonth(cursor.Year, cursor.Month);
    }
}

public sealed class WeeklySchedule : IRecurrenceSchedule
{
    public DayOfWeek Day { get; }
    public WeeklySchedule(DayOfWeek day) => Day = day;
    public IEnumerable<DateOnly> GetOccurrences(DateOnly from, DateOnly to)
    {
        if (to < from) yield break;
        var offset = ((int)Day - (int)from.DayOfWeek + 7) % 7;
        for (var d = from.AddDays(offset); d <= to; d = d.AddDays(7))
            yield return d;
    }
    public DateOnly NextOccurrence(DateOnly after) =>
        GetOccurrences(after.AddDays(1), after.AddDays(7)).First();
}
public enum RecurringKind { Income, Expense }

public sealed class RecurringRule
{
    public string Id { get; }
    public string AccountName { get; }
    public RecurringKind Kind { get; }
    public decimal Amount { get; }
    public Category Category { get; }
    public IRecurrenceSchedule Schedule { get; }
    public DateOnly StartDate { get; }
    public DateOnly? EndDate { get; }
    public bool IsActive { get; set; } = true;
    public DateOnly? LastGeneratedDate { get; internal set; }
    public RecurringRule(string id, string accountName, RecurringKind kind, decimal amount, Category category,
        IRecurrenceSchedule schedule, DateOnly startDate, DateOnly? endDate = null)
    {
        if (string.IsNullOrWhiteSpace(id))
            throw new ArgumentException("Id không được để trống.", nameof(id));
        if (string.IsNullOrWhiteSpace(accountName))
            throw new ArgumentException("Tên tài khoản không được để trống.", nameof(accountName));
        if (amount <= 0)
            throw new ArgumentOutOfRangeException(nameof(amount), amount, "Số tiền phải lớn hơn 0.");
        if (endDate is { } e && e < startDate)
            throw new ArgumentException("EndDate không được trước StartDate.", nameof(endDate));
        Id = id;
        AccountName = accountName.Trim();
        Kind = kind;
        Amount = amount;
        Category = category ?? throw new ArgumentNullException(nameof(category));
        Schedule = schedule ?? throw new ArgumentNullException(nameof(schedule));
        StartDate = startDate;
        EndDate = endDate;
    }
    public ITransaction CreateTransaction(DateOnly date)
    {
        var at = date.ToDateTime(TimeOnly.MinValue);
        return Kind == RecurringKind.Income
            ? new Income(Amount, Category, at)
            : new Expense(Amount, Category, at);
    }
}

public sealed class RecurringGenerator
{
    public IReadOnlyList<DateOnly> GetDueDates(RecurringRule rule, DateOnly upTo)
    {
        if (!rule.IsActive) return Array.Empty<DateOnly>();

        var from = rule.LastGeneratedDate is { } last ? last.AddDays(1) : rule.StartDate;
        if (from < rule.StartDate) from = rule.StartDate;

        var to = rule.EndDate is { } end && end < upTo ? end : upTo;
        if (to < from) return Array.Empty<DateOnly>();

        return rule.Schedule.GetOccurrences(from, to).ToList();
    }
}