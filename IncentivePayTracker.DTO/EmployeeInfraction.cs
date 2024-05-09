namespace IncentivePayTracker.DTO;

public class EmployeeInfraction
{
    public Employee Employee { get; set; } = default!;
    public Infraction Infraction { get; set; } = default!;
    public int Year { get; set; }
    public int Month { get; set; }
    public int Count { get; set; }
}
