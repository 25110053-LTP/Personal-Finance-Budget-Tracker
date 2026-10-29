using BudgetTracker.Recurrence;
namespace BudgetTracker.Models;

public sealed class Ledger
{
    private readonly Dictionary<string, Account> _accounts = new(StringComparer.OrdinalIgnoreCase);
    private readonly List<RecurringRule> _rules = new();
    private readonly RecurringGenerator _generator = new();
    public IReadOnlyCollection<Account> Accounts => _accounts.Values;
    public IReadOnlyList<RecurringRule> Rules => _rules;
    public decimal TotalBalance => _accounts.Values.Sum(a => a.Balance);
    public Account AddAccount(Account account)
    {
        ArgumentNullException.ThrowIfNull(account);
        if (!_accounts.TryAdd(account.Name, account))
            throw new InvalidOperationException($"Tài khoản '{account.Name}' đã tồn tại.");
        return account;
    }
    public Account GetAccount(string name) =>
        _accounts.TryGetValue(name.Trim(), out var a)
            ? a
            : throw new KeyNotFoundException($"Không tìm thấy tài khoản '{name}'.");
    public void AddRule(RecurringRule rule)
    {
        ArgumentNullException.ThrowIfNull(rule);
        GetAccount(rule.AccountName);
        _rules.Add(rule);
    }
    public IReadOnlyList<ITransaction> RunRecurring(DateOnly upTo)
    {
        var due = _rules
            .SelectMany(r => _generator.GetDueDates(r, upTo).Select(d => (Rule: r, Date: d)))
            .OrderBy(x => x.Date)
            .ThenBy(x => x.Rule.Kind)
            .ToList();
        var created = new List<ITransaction>();
        foreach (var (rule, date) in due)
        {
            var tx = rule.CreateTransaction(date);
            GetAccount(rule.AccountName).Record(tx);
            rule.LastGeneratedDate = date;
            created.Add(tx);
        }
        return created;
    }
}