using BudgetTracker.Models;
using Xunit;

namespace BudgetTracker.Tests;

public class CreditCardAccountTests
{
    private const decimal Limit = 10_000_000m;

    private static CreditCardAccount NewCard() => new("Thẻ VISA", Limit);

    [Fact] // CC-01: chi trong hạn mức -> ghi nợ, hạn mức khả dụng giảm
    public void Spend_WithinLimit_CreatesDebt()
    {
        var card = NewCard();

        card.Record(new Expense(3_000_000m, "Mua sắm"));

        Assert.Equal(-3_000_000m, card.Balance);
        Assert.Equal(3_000_000m, card.Debt);
        Assert.Equal(7_000_000m, card.AvailableCredit);
    }

    [Fact] // CC-02: chi đúng bằng hạn mức vẫn được
    public void Spend_ExactlyLimit_IsAllowed()
    {
        var card = NewCard();

        card.Record(new Expense(Limit, "Mua sắm"));

        Assert.Equal(0m, card.AvailableCredit);
        Assert.Equal(Limit, card.Debt);
    }

    [Fact] // CC-03: vượt hạn mức 1 đồng -> bị từ chối, số dư và lịch sử không đổi
    public void Spend_OverLimit_IsRejected_AndNothingChanges()
    {
        var card = NewCard();

        Assert.Throws<InsufficientFundsException>(
            () => card.Record(new Expense(Limit + 1m, "Mua sắm")));

        Assert.Equal(0m, card.Balance);
        Assert.Empty(card.Transactions);
    }

    [Fact] // CC-04: nhiều khoản chi cộng dồn vượt hạn mức
    public void Spend_Cumulative_ExceedsLimit()
    {
        var card = NewCard();
        card.Record(new Expense(6_000_000m, "Mua sắm"));

        Assert.Throws<InsufficientFundsException>(
            () => card.Record(new Expense(5_000_000m, "Du lịch")));

        Assert.Equal(-6_000_000m, card.Balance);
        Assert.Single(card.Transactions);
    }

    [Fact] // CC-05: thanh toán dư nợ (Income) trả lại hạn mức khả dụng
    public void Payment_RestoresAvailableCredit()
    {
        var card = NewCard();
        card.Record(new Expense(8_000_000m, "Mua sắm"));

        card.Record(new Income(5_000_000m, "Thanh toán thẻ"));

        Assert.Equal(3_000_000m, card.Debt);
        Assert.Equal(7_000_000m, card.AvailableCredit);
    }

    [Theory] // CC-06: hạn mức không hợp lệ
    [InlineData(0)]
    [InlineData(-1)]
    public void Constructor_InvalidLimit_Throws(int limit)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new CreditCardAccount("Thẻ", limit));
    }
}