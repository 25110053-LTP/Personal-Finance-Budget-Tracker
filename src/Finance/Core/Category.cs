namespace BudgetTracker.Models;

public sealed record Category
{
    public string Name { get; }

    public Category(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Tên danh mục không được để trống.", nameof(name));

        Name = name.Trim();
    }

    public override string ToString() => Name;
    public static implicit operator Category(string name) => new(name);
}
