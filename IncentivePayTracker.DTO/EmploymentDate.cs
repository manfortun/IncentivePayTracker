namespace IncentivePayTracker.DTO;

public class EmploymentDate
{
    public int Id { get; set; }
    public int EmployeeId { get; set; }
    public int YearHired { get; set; }
    public int MonthHired { get; set; }
    public int? YearTerminated { get; set; }
    public int? MonthTerminated { get; set; } = null;
}
