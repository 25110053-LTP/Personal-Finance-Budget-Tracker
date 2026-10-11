using Xunit;
using BudgetTracker.Models;
using System.Runtime.CompilerServices;

public class PolymorphismTests
{
    [Fact]
    public void Record_DifferentTransactionTypes_CalculatesBalanceCorrectly()
    {
        var account = new Account("Test 4",openingBalance: 1_000_000m);
        ITransaction[] list =
        {
            new Income(500_000m, "thưởng"),
            new Expense(200_000m, "túi cà phê")
        };

        foreach (var t in list)
        {
            account.Record(t);
        }

        Assert.Equal(1_300_000m, account.Balance);
    }
}