namespace BudgetTracker.Models;

public sealed class InvalidAmountException : ArgumentOutOfRangeException
{
    public InvalidAmountException(string paramName, decimal amount)
        : base(paramName, amount, "Số tiền phải lớn hơn 0.") { }
}