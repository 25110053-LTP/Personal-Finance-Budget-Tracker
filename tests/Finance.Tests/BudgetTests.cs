using BudgetTracker.Models;
using Xunit;

public class BudgetTests
{
    private static readonly Period Oct2026 = new(2026, 10);
    private static readonly DateTime InOct = new(2026, 10, 15);

    private static Budget NewBudget() => new("Ăn uống", Oct2026, 1_000_000m);

    private static Expense Spend(decimal amount, string category = "Ăn uống", DateTime? date = null)
        => new(amount, category, date ?? InOct);

    [Fact] // BG-01
    public void Evaluate_SpentBelowWarning_ReturnsOk()
    {
        var list = new List<ITransaction> { Spend(500_000m) };
        Assert.Equal(BudgetStatus.Ok, NewBudget().Evaluate(list));
    }

    [Fact] // BG-02
    public void Evaluate_SpentAtWarningRatio_ReturnsWarning()
    {
        var list = new List<ITransaction> { Spend(800_000m) };
        Assert.Equal(BudgetStatus.Warning, NewBudget().Evaluate(list));
    }

    [Fact] // BG-03: đúng bằng hạn mức vẫn chưa vượt
    public void Evaluate_SpentEqualsLimit_ReturnsWarning()
    {
        var list = new List<ITransaction> { Spend(1_000_000m) };
        Assert.Equal(BudgetStatus.Warning, NewBudget().Evaluate(list));
    }

    [Fact] // BG-04
    public void Evaluate_SpentOverLimit_ReturnsExceeded()
    {
        var list = new List<ITransaction> { Spend(600_000m), Spend(400_001m) };
        Assert.Equal(BudgetStatus.Exceeded, NewBudget().Evaluate(list));
    }

    [Fact] // BG-05: khác category, khác tháng, hoặc là Income thì không tính
    public void Evaluate_IgnoresOtherCategoryOtherPeriodAndIncome()
    {
        var list = new List<ITransaction>
        {
            Spend(900_000m, "Đi lại"),
            Spend(900_000m, date: new DateTime(2026, 9, 30)),
            new Income(900_000m, "Ăn uống", InOct)
        };

        var budget = NewBudget();
        Assert.Equal(0m, budget.Spent(list));
        Assert.Equal(BudgetStatus.Ok, budget.Evaluate(list));
    }

    [Fact] // BG-06
    public void Constructor_NonPositiveLimit_Throws()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new Budget("Ăn uống", Oct2026, 0m));
        Assert.Throws<ArgumentOutOfRangeException>(() => new Budget("Ăn uống", Oct2026, -1m));
    }
}