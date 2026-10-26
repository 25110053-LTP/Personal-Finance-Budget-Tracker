namespace BudgetTracker.Models;

public sealed class Income : TransactionBase
{
    public Income(decimal amount, Category category, DateTime? date = null) : base(amount, category, date){}
    public override void ApplyTo(Account account)
    {
        ArgumentNullException.ThrowIfNull(account);
        account.Deposit(Amount);
    }
}