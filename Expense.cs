namespace BudgetTracker.Models;

public sealed class Expense : TransactionBase
{
    public Expense(decimal amount, Category category, DateTime? date = null) : base(amount, category, date){}
    public override void ApplyTo(Account account)
    {
        ArgumentNullException.ThrowIfNull(account);
        account.Withdraw(Amount);
    }
}