using BudgetTracker.Models;
using BudgetTracker.Recurrence;

// thêm using cho namespace của chúng (xem bước kiểm tra bên dưới).

var account = new Account("Ví chính", openingBalance: 1_000_000m);
Console.WriteLine($"Tạo tài khoản '{account.Name}' - số dư: {account.Balance:N0} đ\n");

// 1. Ghi các giao dịch hợp lệ
Console.WriteLine("== Ghi giao dịch ==");
account.Record(new Income(15_000_000m, "Lương"));
Console.WriteLine($"Sau thu nhập:  {account.Balance:N0} đ");

account.Record(new Expense(3_500_000m, "Tiền nhà"));
account.Record(new Expense(450_000m, "Ăn uống"));
Console.WriteLine($"Sau chi tiêu:  {account.Balance:N0} đ\n");

// 2. Minh họa khoản chi bị từ chối (vượt số dư)
Console.WriteLine("== Khoản chi bị từ chối ==");
try
{
    account.Record(new Expense(50_000_000m, "Mua sắm"));
}
catch (InsufficientFundsException ex)
{
    Console.WriteLine($"Từ chối: {ex.Message}");
}
Console.WriteLine($"Số dư không đổi: {account.Balance:N0} đ\n");

// 3. Validation dùng chung ở TransactionBase
Console.WriteLine("== Validation ==");
TryCreate(() => new Income(-100m, "Thưởng"));
TryCreate(() => new Expense(100_000m, "  "));

// 4. Lịch sử
Console.WriteLine("\n== Lịch sử giao dịch (chỉ gồm giao dịch thành công) ==");
foreach (var t in account.Transactions)
    Console.WriteLine(t);

Console.WriteLine($"\nSố dư cuối: {account.Balance:N0} đ");

// 5. Nhiều tài khoản + thẻ tín dụng
Console.WriteLine("\n== Nhiều tài khoản + thẻ tín dụng ==");
var ledger = new Ledger();
ledger.AddAccount(account);
ledger.AddAccount(new Account("Tiết kiệm", 20_000_000m));
var visa = (CreditCardAccount)ledger.AddAccount(new CreditCardAccount("Visa", 10_000_000m));
 
visa.Record(new Expense(3_000_000m, "Mua sắm"));
Console.WriteLine($"Visa: nợ {visa.Debt:N0} đ, hạn mức còn {visa.AvailableCredit:N0} đ");
try
{
    visa.Record(new Expense(9_000_000m, "Laptop"));
}
catch (InsufficientFundsException ex)
{
    Console.WriteLine($"Từ chối: {ex.Message}");
}
 
// 6. Giao dịch định kỳ
Console.WriteLine("\n== Giao dịch định kỳ (01/2026 - 03/2026) ==");
var start = new DateOnly(2026, 1, 1);
ledger.AddRule(new RecurringRule("SALARY", "Ví chính", RecurringKind.Income, 15_000_000m, "Lương", new MonthlySchedule(5), start));
ledger.AddRule(new RecurringRule("RENT", "Ví chính", RecurringKind.Expense, 3_500_000m, "Tiền nhà", new MonthlySchedule(1), start));
var created = ledger.RunRecurring(new DateOnly(2026, 3, 31));
Console.WriteLine($"Đã sinh {created.Count} giao dịch.");
Console.WriteLine($"Chạy lại: {ledger.RunRecurring(new DateOnly(2026, 3, 31)).Count} giao dịch mới (không bị trùng).");
 
// 7. Số dư các tài khoản và tài sản ròng
Console.WriteLine("\n== Tổng quan ==");
foreach (var a in ledger.Accounts)
    Console.WriteLine($"{a.Name,-12}{a.Balance,15:N0} đ");
 
var nw = new NetWorthSummary(ledger.Accounts);
Console.WriteLine(new string('-', 30));
Console.WriteLine($"{"Tài sản",-12}{nw.TotalAssets,15:N0} đ");
Console.WriteLine($"{"Nợ",-12}{nw.TotalLiabilities,15:N0} đ");
Console.WriteLine($"{"Tài sản ròng",-12}{nw.NetWorth,15:N0} đ");
 
static void TryCreate(Func<ITransaction> factory)
{
    try
    {
        factory();
    }
    catch (ArgumentException ex) // gồm cả ArgumentOutOfRangeException
    {
        Console.WriteLine($"Không hợp lệ: {ex.Message.Split('\n')[0]}");
    }
}  
Console.WriteLine("\n== Báo cáo chi tiêu 03/2026 ==");
var all = ledger.Accounts.SelectMany(a => a.Transactions).ToList();
var report = new SpendingReportService();
var march = new Period(2026, 3);
foreach (var r in report.ByCategory(all, march))Console.WriteLine($"{r.Category,-12}{r.Total,15:N0} đ {r.Percent,6}%");
var trend = new TrendAnalyzer(report).CompareWithPreviousMonth(all, march);
Console.WriteLine($"So với tháng 2: {trend.Direction}, chênh {trend.Change:N0} đ");