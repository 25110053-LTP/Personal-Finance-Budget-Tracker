Giải thích về đa hình

Đa hình trong code được dùng trong interface 'ITrasaction' và phương thức 'ApplyTo(Account account). Hai lớp con 'Income' và 'Expense' đều kế thừa từ 'TransactionBase' nhưng có hoạt động ApplyTo khác nhau. Với Income là cộng còn Expense là trừ.

Minh chứng được sử dụng: Trong lớp 'Account', hàm giao dịch chỉ nhận tham số chung 'ITransaction'.Khi ta gọi `transaction.ApplyTo(this)`, chương trình sẽ tự động biết phải gọi hàm cộng hay trừ tùy thuộc vào đối tượng thực tế được truyền vào là `Income` hay `Expense` lúc chạy. (đã kiểm tra qua file PolymorphismTests.cs)



