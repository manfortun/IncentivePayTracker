namespace IncentivePayTracker.DTO;

public class Employee
{
    public int Id { get; set; }
    public string FirstName { get; set; } = default!;
    public string LastName { get; set; } = default!;
    public bool IsExempted { get; set; }
}
