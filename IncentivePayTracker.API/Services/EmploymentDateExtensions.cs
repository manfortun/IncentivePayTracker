using IncentivePayTracker.API.Models;

namespace IncentivePayTracker.API.Services;

public static class EmploymentDateExtensions
{
    public static bool IsEmployedDuring(this EmploymentDate employmentDate, int year, int? month = null)
    {
        if (employmentDate.YearHired > year) return false;

        if (month.HasValue && employmentDate.MonthHired > month.Value) return false;

        if (employmentDate.YearTerminated.HasValue && employmentDate.YearTerminated < year) return false;

        if (month.HasValue && employmentDate.MonthTerminated.HasValue && employmentDate.MonthTerminated < month.Value) return false;

        return true;
    }
}
