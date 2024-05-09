namespace IncentivePayTracker.DTO;

public class CompositeEmployeeInfraction
{
    public Employee Employee { get; set; } = default!;
    public List<CompositeInfraction>? Infractions { get; set; } = default!;
    public int Year { get; set; }
    public int Month { get; set; }
}
