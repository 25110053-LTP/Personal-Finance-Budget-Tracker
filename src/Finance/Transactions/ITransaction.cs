namespace BudgetTracker.Models;

public interface ITransaction
{
    Guid Id{ get; }
    decimal Amount{ get; }
    Category Category{ get; }
    DateTime Date{ get; }
    void ApplyTo(Account account);
}