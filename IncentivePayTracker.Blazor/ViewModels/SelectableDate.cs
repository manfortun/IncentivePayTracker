namespace IncentivePayTracker.Blazor.ViewModels;

public class SelectableDate
{
    public int Month { get; init; }
    public string Value { get; init; }
    public int Year { get; init; }

    public SelectableDate(int key, string value, int year)
    {
        Month = key;
        Value = value;
        Year = year;
    }

    public SelectableDate(KeyValuePair<int, string> monthKvp, int year)
    {
        Month = monthKvp.Key;
        Value = monthKvp.Value;
        Year = year;
    }

    public DateTime ToDateTime()
    {
        return new DateTime(Year, Month, 1);
    }

    public DateOnly ToDateOnly()
    {
        return DateOnly.FromDateTime(ToDateTime());
    }
}
