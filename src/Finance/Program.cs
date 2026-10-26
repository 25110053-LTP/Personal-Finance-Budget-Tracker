using System;
using BudgetTracker.Models;

namespace BudgetTracker
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var account = new Account("Ví chính", openingBalance: 1_000_000m);
            Console.WriteLine($"Tạo tài khoản '{account.Name}' - số dư: {account.Balance:N0} đ\n");

            Console.WriteLine("\n== Lịch sử giao dịch (chỉ gồm giao dịch thành công) ==");
            foreach (var t in account.Transactions) Console.WriteLine(t);
            Console.WriteLine($"\nSố dư cuối: {account.Balance:N0} đ");
        }

        static void TryCreate(Func<ITransaction> factory)
        {
            try
            {
                factory();
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"Không hợp lệ: {ex.Message.Split('\n')[0]}");
            }
        }
    }
}