namespace BudgetTracker.Models;
public abstract class TransactionBase : ITransaction
{
    public Guid Id{ get; } = Guid.NewGuid();
    public decimal Amount{ get; }
    public Category Category{ get; }
    public DateTime Date{ get; }
    protected TransactionBase(decimal amount, Category category, DateTime? date = null)
    {
        if(amount <= 0) throw new InvalidAmountException(nameof(amount), amount);
        ArgumentNullException.ThrowIfNull(category);
        Amount = amount;
        Category = category;
        Date = date ?? DateTime.Now;
    }
    public abstract void ApplyTo(Account account);

    public override string ToString() => $"{GetType().Name,-8}{Amount,15:N0} đ  {Category}  {Date:dd/MM/yyyy}";
}