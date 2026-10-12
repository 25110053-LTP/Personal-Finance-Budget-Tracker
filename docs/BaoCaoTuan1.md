1. Mục tiêu tuần 1
Chốt thiết kế và công nghệ, xây dựng phần model lõi (Account, Category, ITransaction với Income/Expense) và kiểm thử được các hành vi cơ bản, trong đó có điểm Polymorphism.
2. Công việc đã hoàn thành
- Quyết định kỹ thuật: Chọn ngôn ngữ C# (.NET 10), build và test bằng dotnet CLI với xUnit. Tạo repo Git chung. (Cả nhóm, xong)
- Phân tích đề: Viết lại bài toán (Problem restatement), liệt kê 5 yêu cầu chức năng và 3 nguyên lý OOP cần thể hiện. (Cả nhóm, xong)
- Class diagram v1: Bản nháp các lớp lõi và quan hệ giữa chúng (xem mục 3). (Phước, xong)
- Category, Account: Category bất biến, tên không được trống. Account quản lý số dư và lịch sử giao dịch, kiểm tra không cho chi vượt số dư (overdraft) và ném InsufficientFundsException. (Phước, xong)
- ITransaction, TransactionBase, Income, Expense: Interface ITransaction với phương thức ApplyTo(Account); lớp abstract TransactionBase giữ dữ liệu và validation dùng chung (số tiền > 0, bắt buộc có category); Income cộng số dư, Expense trừ số dư. (Thư, xong)
- Demo console: Program.cs: tạo account, ghi giao dịch, in số dư, minh họa một khoản chi bị từ chối. (Thư, xong)
- Kiểm thử: Dùng xUnit. A viết AccountTests (happy path, overdraft, validation); PolymorphismTests (đa hình Income/Expense). (Phước, xong)
- Tài liệu: Dựng khung báo cáo cuối, viết README build/run (A), bắt đầu AI Usage Log và nháp Self-Explanation cho Polymorphism (B). (A và B, xong)
3. Thiết kế đã chốt (Class diagram v1)
          <<interface>>
           ITransaction
     + ApplyTo(Account)
            ^
            |
     TransactionBase (abstract)
     + Amount, Date, Category
        ^           ^
        |           |
     Income       Expense
  (cong so du)  (tru so du)

  Account 1 ------- * ITransaction
  ITransaction * -- 1 Category
  Account --> InsufficientFundsException

Quyết định thiết kế: 
- Income và Expense cùng cài đặt interface ITransaction thông qua lớp abstract TransactionBase. 
- TransactionBase giữ dữ liệu và validation dùng chung để không lặp code; ApplyTo() để Income và Expense tự quyết định ảnh hưởng lên số dư. 
- Account và service chỉ làm việc với kiểu ITransaction, không cần if (isIncome), và sau này thêm loại giao dịch mới sẽ không phải sửa các lớp đang có.
4. Minh chứng kiểm thử
Happypath: Tạo account số dư 0, ghi Income 1.000.000 rồi Expense 300.000, kết quả: Số dư = 700.000, có 2 giao dịch
Validation: Ghi Expense lớn hơn số dư	Validation	Ném InsufficientFundsException, số dư và lịch sử không đổi	
Validation: Tạo giao dịch với số tiền ≤ 0, hoặc không có category, Kết quả: bị từ chối với ngoại lệ tương ứng
Polymorphism: Duyệt một danh sách ITransaction gồm Income và Expense, gọi ApplyTo(), kết quả: cùng một lời gọi, số dư tăng với Income và giảm với Expense

5. Khó khăn và quyết định
Tuần 1 nhóm không gặp khó khăn đáng kể. Các quyết định thiết kế chính:
•	Dùng interface ITransaction kết hợp abstract class TransactionBase: interface để Account và service chỉ phụ thuộc vào kiểu ITransaction, abstract class để Income và Expense không lặp code validation.
•	Kiểm tra overdraft ở Account.Withdraw vì Account là nơi giữ số dư; kiểm tra số tiền hợp lệ và category bắt buộc ở constructor của giao dịch.
•	Deposit và Withdraw của Account để internal, số dư chỉ thay đổi được qua Account.Record().
•	Dùng decimal để tránh sai số khi tính tiền.
- Tuần đầu tiên chưa cập nhập rubric mà làm riêng bên ngoài bằng vscode, có khả năng vi phạm rubric, quyết định cập nhập kịp thời tiến độ mới nhất tránh ảnh hưởng đến các nhiệm vụ sau này.
6. Cập nhật AI Usage Log (tuần 1)
- [5/10] Claude:
Prompt: Lập kế hoạch 4 tuần cho đề tài	    
AI result: Kế hoạch theo tuần và mẫu báo cáo	
Adjustment: Giữ khung kế hoạch và phân công A/B
- [5/10] Claude:
Prompt: Soạn mẫu báo cáo tuần 1 (file Word)	
AI result: Bản báo cáo có các mục theo mẫu	
Adjustment: Sửa lại bài mẫu ở các phần “Quyết định thiết kế quan trọng”, “4. Minh chứng kiểm thử”, “Khó khăn và quyết định”,  sửa lại thông tin thực tế của nhóm, tự chạy test để ghi kết quả, không sử dụng file wold.
- [10/10] Claude:
Prompt: error MSB4044: The "GetRestoreProjectStyleTask" task was not given a value for the required parameter "MSBuildProjectName". là lỗi gì	
AI result: Các cách xử lý lỗi	
Adjustment: Gọi trực tiếp tiếp file finance.csproj. Build thành công
[11/10]	Claude	
Prompt: Cách tải xUnit	
AI result: Hướng dẫn cài đặt	
Adjustment: Cài đặt thành công

7. Kế hoạch tuần 2
•	A: Budget theo category và theo kỳ (tháng), tự theo dõi và tự đánh giá trạng thái (OK / gần ngưỡng / vượt) qua status().
•	B: hỗ trợ nhiều account (Checking, Credit Card), NetWorthSummary, TransactionService điều phối ghi giao dịch.
•	Chung: test cho vượt ngân sách và net worth, viết Self-Explanation cho Encapsulation, cập nhật class diagram v2.
