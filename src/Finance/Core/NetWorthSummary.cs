namespace BudgetTracker.Models;

public sealed class NetWorthSummary
{
    public decimal TotalAssets{get;}
    public decimal TotalLiabilities{get;}
    public decimal NetWorth => TotalAssets - TotalLiabilities;

    public NetWorthSummary(IEnumerable<Account> accounts)
    {
        ArgumentNullException.ThrowIfNull(accounts);
        var list = accounts.ToList();

        TotalAssets = list.Where(a => a.Balance > 0).Sum(a => a.Balance);
        TotalLiabilities = list.Where(a => a.Balance < 0).Sum(a => -a.Balance);
    }
}