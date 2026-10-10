namespace BudgetTracker.Models;

public sealed class Account
{
    private readonly List<ITransaction> _transactions = new();

    public string Name { get; }
    public decimal Balance { get; private set; }
    public IReadOnlyList<ITransaction> Transactions => _transactions;

    public Account(string name, decimal openingBalance = 0m)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Tên tài khoản không được để trống.", nameof(name));
        if (openingBalance < 0)
            throw new ArgumentOutOfRangeException(nameof(openingBalance), "Số dư ban đầu không được âm.");

        Name = name.Trim();
        Balance = openingBalance;
    }

    //Ghi giao dịch
    public void Record(ITransaction transaction)
    {
        ArgumentNullException.ThrowIfNull(transaction);

        transaction.ApplyTo(this);      
        _transactions.Add(transaction);
    }

    internal void Deposit(decimal amount)
    {
        if (amount <= 0)
            throw new ArgumentOutOfRangeException(nameof(amount), "Số tiền phải lớn hơn 0.");

        Balance += amount;
    }

    internal void Withdraw(decimal amount)
    {
        if (amount <= 0)
            throw new ArgumentOutOfRangeException(nameof(amount), "Số tiền phải lớn hơn 0.");
        if (amount > Balance)
            throw new InsufficientFundsException(Name, Balance, amount);

        Balance -= amount;
    }
}
