using BudgetTracker.Models;

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
foreach (var t in account.Transactions) Console.WriteLine(t);
Console.WriteLine($"\nSố dư cuối: {account.Balance:N0} đ");

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

// 5. happy path
Console.WriteLine("\n== happy path ==");
var acc1 = new Account("Test 1");
acc1.Record(new Income(1_000_000m, "Lương"));
acc1.Record(new Expense(300_000m, "Ăn uống"));
Console.WriteLine($"Số dư: {acc1.Balance:N0} đ, số giao dịch: {acc1.Transactions.Count}");

// 6. đa hình
Console.WriteLine("\n== đa hình ==");
var acc4 = new Account("Test 4", 1_000_000m);
ITransaction[] list = { new Income(500_000m, "Thưởng"), new Expense(200_000m, "Cà phê") };
foreach (var t in list)
{
    var before = acc4.Balance;
    acc4.Record(t);
    Console.WriteLine($"{t.GetType().Name,-8}: {before:N0} -> {acc4.Balance:N0} đ");
}