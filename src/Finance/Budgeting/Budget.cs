namespace BudgetTracker.Models;

public sealed class Budget
{
    public Category Category {get;}
    public Period Period {get;}
    public decimal Limit {get;}
    public decimal WarningRatio {get;}

    public Budget(Category category, Period period, decimal limit, decimal warningRatio = 0.8m)
    {
        ArgumentNullException.ThrowIfNull(category);
        if (limit <= 0) throw new ArgumentOutOfRangeException(nameof(limit), limit, "Hạn mức phải lớn hơn 0");
        if (warningRatio is <= 0m or > 1m) throw new ArgumentOutOfRangeException(nameof(warningRatio), warningRatio, "Tỉ lệ cảnh báo phải ở trong khoảng (0, 1]");

        Category = category;
        Period = period;
        Limit = limit;
        WarningRatio = warningRatio;
    }

    //Tổng chi
    public decimal Spent(IEnumerable<ITransaction> transactions)
    {
        return transactions
            .OfType<Expense>()
            .Where(e => e.Category == Category && Period.Contains(e.Date))
            .Sum(e => e.Amount);
    }

    public BudgetStatus Evaluate(IEnumerable<ITransaction> transactions)
    {
        var spent = Spent(transactions);

        if (spent > Limit) return BudgetStatus.Exceeded;
        if (spent >= Limit * WarningRatio) return BudgetStatus.Warning;
        return BudgetStatus.OK;
    }
}