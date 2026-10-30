using BudgetTracker.Models;
using Xunit;

namespace BudgetTracker.Tests;

// VA-01 (chi quá số dư) và VA-02 (số tiền <= 0) đã có trong AccountTests.
// Lỗi tài khoản trùng tên và rule trỏ tài khoản không tồn tại đã có trong RecurringTests (LG-01).
public class ValidationTests
{
    [Fact] // VA-03: tên danh mục trống bị từ chối
    public void BlankCategoryName_IsRejected()
    {
        Assert.Throws<ArgumentException>(() => new Category("   "));
        Assert.Throws<ArgumentException>(() => new Expense(100_000m, "  "));
    }

    [Fact] // VA-04: ghi giao dịch null bị từ chối, tài khoản không đổi
    public void RecordingNull_IsRejected()
    {
        var account = new Account("Ví", 100_000m);

        Assert.Throws<ArgumentNullException>(() => account.Record(null!));

        Assert.Equal(100_000m, account.Balance);
        Assert.Empty(account.Transactions);
    }
}
