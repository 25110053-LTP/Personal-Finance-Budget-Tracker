# Class Diagram v2 (Tuần 2)

Phạm vi: `Budget`, giao dịch định kỳ, nhiều tài khoản qua `Ledger`.
Báo cáo, trend, net worth: dự kiến v3.

## Sơ đồ

```mermaid
classDiagram
    direction TB

    class ITransaction {
        <<interface>>
        +ApplyTo(Account account) void
    }
    class TransactionBase {
        <<abstract>>
        +decimal Amount
        +DateOnly Date
        +Category Category
        +string SourceKey
    }
    class Income {
        +ApplyTo(Account account) void
    }
    class Expense {
        +ApplyTo(Account account) void
    }
    class Category {
        +string Name
    }
    class Account {
        +string Name
        +decimal Balance
        +IReadOnlyList~ITransaction~ Transactions
        +Post(ITransaction tx) void
    }
    class Ledger {
        +IReadOnlyList~Account~ Accounts
        +AddAccount(Account account) void
        +AddBudget(Budget budget) void
        +Post(Account account, ITransaction tx) void
        +Contains(string sourceKey) bool
    }
    class Period {
        +int Year
        +int Month
        +Contains(DateOnly date) bool
    }
    class Budget {
        -decimal spent
        +Category Category
        +Period Period
        +decimal Limit
        +Record(Expense expense) void
        +Evaluate() BudgetStatus
    }
    class BudgetStatus {
        <<enumeration>>
        OK
        Warning
        Exceeded
    }
    class IRecurrenceSchedule {
        <<interface>>
        +Occurrences(DateOnly from, DateOnly to) IEnumerable~DateOnly~
    }
    class MonthlySchedule {
        +int DayOfMonth
    }
    class WeeklySchedule {
        +DayOfWeek Day
    }
    class RecurringRule {
        +string Id
        +Account Account
        +TransactionBase Template
        +IRecurrenceSchedule Schedule
    }
    class RecurringGenerator {
        +Generate(RecurringRule rule, DateOnly from, DateOnly to) int
    }
    class InsufficientFundsException
    class InvalidAmountException

    ITransaction <|.. TransactionBase
    TransactionBase <|-- Income
    TransactionBase <|-- Expense
    IRecurrenceSchedule <|.. MonthlySchedule
    IRecurrenceSchedule <|.. WeeklySchedule

    TransactionBase "*" --> "1" Category
    Account "1" o-- "*" ITransaction
    Ledger "1" o-- "*" Account
    Ledger "1" o-- "*" Budget
    Budget "*" --> "1" Category
    Budget "*" --> "1" Period
    Budget ..> BudgetStatus : returns
    RecurringRule --> IRecurrenceSchedule
    RecurringRule --> TransactionBase : template
    RecurringRule --> Account
    RecurringGenerator ..> RecurringRule : reads
    RecurringGenerator ..> Ledger : posts via
    Account ..> InsufficientFundsException : throws
    TransactionBase ..> InvalidAmountException : throws
```

## Thay đổi so với v1

| Hạng mục mới | Vai trò | Thành viên | File |
|---|---|---|---|
| `Budget`, `BudgetStatus`, `Period` | Theo dõi chi theo category theo kỳ, tự đánh giá trạng thái | A (Phước) | [link file] |
| `InvalidAmountException` | Từ chối số tiền không hợp lệ | A (Phước) | [link file] |
| `IRecurrenceSchedule`, `MonthlySchedule`, `WeeklySchedule` | Điểm mở rộng cho lịch định kỳ | B (Thư) | [link file] |
| `RecurringRule`, `RecurringGenerator` | Sinh giao dịch theo khoảng ngày | B (Thư) | [link file] |
| `Ledger` | Quản lý nhiều `Account` và `Budget`; kiểm tra trùng qua `Contains` | B (Thư) | [link file] |
| `TransactionBase.SourceKey` | Khóa `RuleId:OccurrenceDate`, dùng chống sinh trùng | B (Thư) | [link file] |
\