namespace IncentivePayTracker.DTO;

public class EmployeeCompleteInformation
{
    public Employee Employee { get; set; }
    public List<EmploymentDate> EmploymentDates { get; set; } = new List<EmploymentDate>();
    public List<EmployeeTimeIn> TimeIns { get; set; } = new List<EmployeeTimeIn>();
}
