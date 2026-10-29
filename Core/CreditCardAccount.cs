namespace BudgetTracker.Models;

/// <summary>
/// Thẻ tín dụng: Balance âm = đang nợ. Được chi tối đa đến hạn mức (CreditLimit).
/// Income (Deposit) đóng vai trò thanh toán dư nợ.
/// </summary>
public sealed class CreditCardAccount : Account
{
    public decimal CreditLimit{get;}
    public decimal Debt => Balance < 0 ? -Balance : 0m;
    public decimal AvailableCredit => Balance + CreditLimit;

    public CreditCardAccount(string name, decimal creditLimit) : base(name, 0m)
    {
        if(creditLimit <= 0)
            throw new ArgumentOutOfRangeException(nameof(creditLimit), "Hạn mức phải lớn hơn 0.");
        CreditLimit = creditLimit;
    }
    internal override void Withdraw(decimal amount)
    {
        if(amount < 0)
            throw new ArgumentOutOfRangeException(nameof(amount), "Số tiền phải lớn hơn 0.");
        if(amount > AvailableCredit)
            throw new InsufficientFundsException(Name, AvailableCredit, amount);
        Balance -= amount;
    }
}