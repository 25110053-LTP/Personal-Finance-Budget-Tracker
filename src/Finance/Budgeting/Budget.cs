using System.Diagnostics.Contracts;
using BudgetTracker.Models;

namespace BudgetTracker.Model;

public sealed class Budget
{
    public Category Category {get;}
    public Period Period {get;}
    public decimal Limit {get;}
    public decimal WarningRatio {get;}

    public Budget(Category category, Period period, decimal limit, decimal warningRatio = 0.8m)
    {
        if (limit <= 0) throw new ArgumentOutOfRangeException(nameof(limit), limit, "Hạn mức phải lớn hơn 0");
        if (warningRatio is <= 0m or > 1m) throw new ArgumentOutOfRangeException(nameof(warningRatio), warningRatio, "Tỉ lệ cảnh báo phải ở trong khoảng (0, 1]");

        Category = category;
        Period = period;
        Limit = limit;
        WarningRatio = warningRatio;
    }
}