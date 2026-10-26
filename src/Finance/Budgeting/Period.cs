namespace BudgetTracker.Model;

public readonly record struct Period
{
    public int Year {get;}
    public int Month {get;}

    public Period(int Year, int Month)
    {
        if (Year < 1) throw new ArgumentOutOfRangeException(nameof(Year), Year, "Năm không hợp lệ");
        if (Month is < 1 or >12) throw new ArgumentOutOfRangeException(nameof(Month), Month, "Tháng không hợp lệ.");
    }
}