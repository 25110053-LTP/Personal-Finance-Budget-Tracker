namespace BudgetTracker.Models;

public sealed class InsufficientFundsException : InvalidOperationException
{
    public string AccountName { get; }
    public decimal Balance { get; }
    public decimal Requested { get; }

    public InsufficientFundsException(string accountName, decimal balance, decimal requested)
        : base($"Tài khoản '{accountName}' không đủ số dư: hiện có {balance:N0}, cần {requested:N0}.")
    {
        AccountName = accountName;
        Balance = balance;
        Requested = requested;
    }
}
