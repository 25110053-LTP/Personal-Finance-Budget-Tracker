NỘI DUNG BÁO CÁO

BÁO CÁO TIẾN ĐỘ TUẦN 1 — ĐỒ ÁN CUỐI KỲ Môn học: Lập trình Hướng đối tượng (Object-Oriented Programming) Mã lớp học phần: OOP, HK1 2026-2027, Đợt 1 — GVHD: Huỳnh Xuân Phụng Đề tài: Personal Finance / Budget Tracker Link GitHub: (https://github.com/25110053-LTP/Personal-Finance-Budget-Tracker/tree/main)

## Phân công thành viên (Tuần 1)

| Thành viên | MSSV | Nhiệm vụ tuần 1 | File phụ trách |
|---|---|---|---|
| A – Phước | 25110053 | Model cốt lõi (Category, Account, xử lý overdraft) và toàn bộ kiểm thử tự động (xUnit) | [Category.cs](https://github.com/25110053-LTP/Personal-Finance-Budget-Tracker/blob/main/Category.cs), [Account.cs](https://github.com/25110053-LTP/Personal-Finance-Budget-Tracker/blob/main/Account.cs), [InsufficientFundsException.cs](https://github.com/25110053-LTP/Personal-Finance-Budget-Tracker/blob/main/InsufficientFundsException.cs), [AccountTests.cs](https://github.com/25110053-LTP/Personal-Finance-Budget-Tracker/blob/main/AccountTests.cs), [PolymorphismTests.cs](https://github.com/25110053-LTP/Personal-Finance-Budget-Tracker/blob/main/PolymorphismTests.cs) |
| B – Thư | 25110062 | Đa hình giao dịch (ITransaction, TransactionBase, Income, Expense) và kịch bản Demo | [ITransaction.cs](https://github.com/25110053-LTP/Personal-Finance-Budget-Tracker/blob/main/ITransaction.cs), [TransactionBase.cs](https://github.com/25110053-LTP/Personal-Finance-Budget-Tracker/blob/main/TransactionBase.cs), [Income.cs](https://github.com/25110053-LTP/Personal-Finance-Budget-Tracker/blob/main/Income.cs), [Expense.cs](https://github.com/25110053-LTP/Personal-Finance-Budget-Tracker/blob/main/Expense.cs), [Program.cs](https://github.com/25110053-LTP/Personal-Finance-Budget-Tracker/blob/main/Program.cs) |
| Cả hai | | Báo cáo, Class Diagram v1, Self-Explanation | [docs/](https://github.com/25110053-LTP/Personal-Finance-Budget-Tracker/tree/main/docs) |

**Công việc đã thực hiện**

- Thiết kế và cài đặt các class cốt lõi cho phần Model:
  - `Category`: Thiết kế bất biến, kiểm tra tên danh mục không được để trống.
  - `Account`: Quản lý số dư và lịch sử giao dịch. Kiểm tra không cho chi vượt số dư (overdraft) và ném lỗi `InsufficientFundsException`.
  - `ITransaction`, `TransactionBase`, `Income`, `Expense`: Khởi tạo Interface `ITransaction` với phương thức `ApplyTo(Account)`. Lớp abstract `TransactionBase` giữ dữ liệu và validation dùng chung (tiền > 0, bắt buộc có category). `Income` cộng số dư, `Expense` trừ số dư.
- Thể hiện tính Đa hình (Polymorphism) bằng cách cho `Income` và `Expense` cùng cài đặt interface `ITransaction`. Gọi cùng một hàm `ApplyTo()` nhưng tự quyết định cách ảnh hưởng lên số dư (tăng hoặc giảm).
- Quyết định thiết kế để `Account` và Service chỉ làm việc qua kiểu `ITransaction` (không cần dùng if/else kiểm tra isIncome), đảm bảo dễ mở rộng sau này.
- Cài đặt kịch bản kiểm thử Demo qua `Program.cs`: tạo tài khoản, ghi giao dịch thành công, in số dư và minh họa từ chối khoản chi quá tay.
- Viết kịch bản kiểm thử tự động với xUnit: `AccountTests.cs` (happy path, overdraft, validation) và `PolymorphismTests.cs` (đa hình).
- Lên bản nháp sơ đồ lớp Class Diagram v1 và chuẩn bị tài liệu Self-Explanation cho tính đa hình.

**Kết quả**

- Project C# (.NET 10) build và chạy ổn định trên console, quản lý mã nguồn chung qua Git.
- Hệ thống tính đúng số dư, thực hiện giao dịch thu/chi chính xác thông qua tính Đa hình.
- Hệ thống chặn và ném lỗi ngoại lệ chính xác khi giao dịch không hợp lệ (số tiền ≤ 0, không có tên danh mục) hoặc khi chi vượt hạn mức số dư.
- Chạy test tự động bằng lệnh `dotnet test` thành công (Passed 4/4 test cases).

**Khó khăn**

- Tuần đầu tiên do chưa cập nhật rubric nên nhóm làm code riêng bên ngoài bằng VS Code. Có khả năng vi phạm rubric nên quyết định cập nhật kịp thời tiến độ mới nhất lên hệ thống tránh ảnh hưởng đến các nhiệm vụ sau.

**Kế hoạch tuần sau**

- A (Phước): Phát triển tính năng Budget theo Category và theo kỳ (tháng), tự theo dõi và tự đánh giá trạng thái (OK / gần ngưỡng / vượt) qua hàm `status()`.
- B (Thư): Hỗ trợ mở rộng nhiều loại Account (Checking, Credit Card), thêm chức năng `NetWorthSummary` và `TransactionService` để điều phối ghi giao dịch.
- Viết kịch bản test bổ sung cho các trường hợp vượt ngân sách và net worth.
- Viết phần Tự giải trình cho nguyên lý Đóng gói (Encapsulation).
- Vẽ và cập nhật sơ đồ lớp Class Diagram v2.
