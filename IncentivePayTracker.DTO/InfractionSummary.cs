namespace IncentivePayTracker.DTO;

public class InfractionSummary
{
    public int EmployeeId { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public bool IsExempted { get; set; }
    public double TotalInfractionsAmount { get; set; }
}
