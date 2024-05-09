using IncentivePayTracker.Blazor.ViewModels;
using IncentivePayTracker.DTO;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using System.Net;

namespace IncentivePayTracker.Blazor.Services;

public static class DateExtensions
{
    private static Dictionary<int, string> _months = new Dictionary<int, string>()
    {
        { 1, "January" },
        { 2, "February" },
        { 3, "March" },
        { 4, "April" },
        { 5, "May" },
        { 6, "June" },
        { 7, "July" },
        { 8, "August" },
        { 9, "September" },
        { 10, "October" },
        { 11, "November" },
        { 12, "December" },
    };

    public const int MIN_SYSTEM_YEAR = 2023;
    public static readonly int MAX_SYSTEM_YEAR = DateTime.UtcNow.Year;
    public static Dictionary<int, string> Months => _months;

    public static string DateHired(this EmploymentDate ed)
    {
        var date = new DateTime(ed.YearHired, ed.MonthHired, 1, 0, 0, 0, DateTimeKind.Utc);

        return date.ToString("MMM yyyy");
    }

    public static string DateTerminated(this EmploymentDate ed)
    {
        if (ed.YearTerminated.HasValue && ed.MonthTerminated.HasValue)
        {
            var date = new DateTime(ed.YearTerminated.Value, ed.MonthTerminated.Value, 1, 0, 0, 0, DateTimeKind.Utc);
            return date.ToString("MMM yyyy");
        }
        else
        {
            return "Still employed";
        }
    }

    public static IEnumerable<SelectableDate> AvailableEmploymentDates(this List<EmploymentDate> employmentDates, EmploymentDate ed)
    {
        var availableDates = GetSelectableDatesToPresent();

        // remove dates that belong to other employment dates
        // (the employee cannot be reemployed at a time when he/she was currently employed)
        foreach (var date in employmentDates)
        {
            if (ed.Equals(date))
            {
                if (date.YearTerminated.HasValue && date.MonthTerminated.HasValue)
                {
                    availableDates.RemoveAll(d => d.ToDateTime() >= new DateTime(date.YearTerminated.Value, date.MonthTerminated.Value, 1));
                }
            }
            else
            {
                int month = date.MonthTerminated ?? DateTime.UtcNow.Month;
                int year = date.YearTerminated ?? DateTime.UtcNow.Year;
            
                availableDates.RemoveAll(d => d.ToDateTime() >= new DateTime(date.YearHired, date.MonthHired, 1) && d.ToDateTime() <= new DateTime(year, month, 1));
            }
        }

        return availableDates;
    }

    public static List<SelectableDate> AvailableTerminationDates(this List<EmploymentDate> employmentDates, EmploymentDate ed)
    {
        var availableDates = GetSelectableDatesToPresent();
        
        // remove dates where the employee was hired
        // (the employee cannot both be hired and terminated on the same month)
        foreach (var date in employmentDates)
        {
            // remove dates before and on the hiring date
            if (date.Equals(ed))
            {
                availableDates.RemoveAll(d => d.ToDateTime() < new DateTime(date.YearHired, date.MonthHired, 1));
            }
            // if the date occurs after the hiring date
            else if(new DateTime(date.YearHired, date.MonthHired, 1) > new DateTime(ed.YearHired, ed.MonthHired, 1))
            {
                availableDates.RemoveAll(d => d.ToDateTime() >= new DateTime(date.YearHired, date.MonthHired, 1));
            }
        }

        return availableDates;
    }

    public static int[] GetAvailableYears(int? min = null, int? max = null)
    {
        min ??= MIN_SYSTEM_YEAR;
        max ??= MAX_SYSTEM_YEAR;

        return Enumerable.Range(min.Value, max.Value - min.Value + 1).ToArray();
    }

    public static List<SelectableDate> GetSelectableDatesToPresent()
    {
        var availableDates = new List<SelectableDate>();
        var maxDate = GetCurrentDate();

        foreach (var currYear in GetAvailableYears())
        {
            Months.ToList().ForEach(m =>
            {
                var date = new SelectableDate(m, currYear);

                if (date.ToDateTime() <= maxDate)
                {
                    availableDates.Add(new SelectableDate(m, currYear));
                }
            });
        }

        return availableDates;
    }

    public static DateTime GetCurrentDate()
    {
        return new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1);
    }
}
