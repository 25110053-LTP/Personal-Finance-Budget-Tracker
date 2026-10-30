using BudgetTracker.Models;
using Xunit;

namespace BudgetTracker.Tests;

public class NetWorthSummaryTests
{
    [Fact] // NW-01: tài sản = số dư dương, nợ = số dư âm, ròng = tài sản - nợ
    public void Summary_SplitsAssetsAndLiabilities()
    {
        var wallet = new Account("Ví chính", 5_000_000m);
        var card = new CreditCardAccount("Thẻ VISA", 10_000_000m);
        card.Record(new Expense(2_000_000m, "Mua sắm"));

        var s = new NetWorthSummary(new Account[] { wallet, card });

        Assert.Equal(5_000_000m, s.TotalAssets);
        Assert.Equal(2_000_000m, s.TotalLiabilities);
        Assert.Equal(3_000_000m, s.NetWorth);
    }

    [Fact] // NW-02: không có tài khoản nào
    public void Summary_Empty_IsZero()
    {
        var s = new NetWorthSummary(Array.Empty<Account>());

        Assert.Equal(0m, s.NetWorth);
    }
}