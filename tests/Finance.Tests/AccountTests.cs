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
        Assert.Equal(12_050_000m, account.Balance);
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
        Assert.Throws<InvalidAmountException>(() => new Income(-100m, "Thưởng"));
        Assert.Throws<ArgumentException>(() => new Expense(100_000m, " ")); 
    }
    [Fact] // VA-01
    public void Record_ExpenseOverBalance_ThrowsAndKeepsState()
    {
        var acc = new Account("Ví", 100_000m);
        var ex = Assert.Throws<InsufficientFundsException>(() => acc.Record(new Expense(150_000m, "Mua sắm")));

    Assert.Equal("Ví", ex.AccountName);
    Assert.Equal(100_000m, ex.Balance);
    Assert.Equal(150_000m, ex.Requested);
    Assert.Equal(100_000m, acc.Balance);
    Assert.Empty(acc.Transactions);
}

    [Theory] // VA-02
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-500_000)]
    public void CreateTransaction_NonPositiveAmount_ThrowsInvalidAmount(int amount)
    {
        Assert.Throws<InvalidAmountException>(() => new Income(amount, "Lương"));
        Assert.Throws<InvalidAmountException>(() => new Expense(amount, "Ăn uống"));
    }
}

