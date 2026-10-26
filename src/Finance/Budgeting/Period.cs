namespace BudgetTracker.Models;

public readonly record struct Period
{
    public int Year {get;}
    public int Month {get;}

    public Period(int year, int month)
    {
        if (year < 1) throw new ArgumentOutOfRangeException(nameof(year), year, "Năm không hợp lệ");
        if (month is < 1 or >12) throw new ArgumentOutOfRangeException(nameof(month), month, "Tháng không hợp lệ.");

        Year = year;
        Month = month;
    }

    public static Period Of(DateTime date) => new(date.Year, date.Month);
    public bool Contains(DateTime date) => date.Year == Year && date.Month == Month;
    public override string ToString() => $"{Month:00}/{Year}";
}