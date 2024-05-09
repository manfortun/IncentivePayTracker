namespace IncentivePayTracker.API.Models;

public class EmployeeTimeIn
{
    public int EmployeeId { get; set; }
    public virtual Employee Employee { get; set; } = default!;
    public TimeOnly TimeIn { get; set; }
    public int Month { get; set; }
    public int Year { get; set; }
}
