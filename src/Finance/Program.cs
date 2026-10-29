using BudgetTracker.Models;
// Nếu build báo thiếu Income, Expense, ITransaction hoặc InsufficientFundsException,
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