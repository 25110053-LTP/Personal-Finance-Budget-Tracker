using Xunit;
using BudgetTracker.Models;
using System;

public class AccountTests
{
    [Fact]
    public void Record_ValidTransactions_UpdatesBalanceCorrectly()
    {
        //happy path
        var account = new Account("Ví chính", openingBalance: 1_000_000m);

        account.Record(new Income(15_000_000m, "Lương"));
        account.Record(new Expense(3_500_000m, "Tiền nhà"));
        account.Record(new Expense(450_000m, "Ăn uống"));

        //Kiểm tra số dư
        Assert.Equal(12_050_000, account.Balance);
        Assert.Equal(3, account.Transactions.Count);
    }

    [Fact]
    public void Record_ExpenseExceedBalance_ThrowInsufficientFundsException()
    {
        //từ chối
        var account = new Account("Ví chính", openingBalance: 1_000_000m);
        var hughExpense = new Expense(50_000_000, "Mua sắm");

        //kiểm tra lỗi
        Assert.Throws<InsufficientFundsException>(() => account.Record(hughExpense));

        //kiểm tra số dư
        Assert.Equal(1_000_000m, account.Balance);
    }
    
    [Fact]
    public void Transaction_InvalidArguments_ThrowsArgumentException()
    {
        //Validation dùng chung
        Assert.Throws<ArgumentException>(() => new Income(-100m, "Thưởng"));
        Assert.Throws<ArgumentException>(() => new Expense(100_000m, " ")); 
    }
}