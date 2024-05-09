using System.ComponentModel.DataAnnotations;

namespace IncentivePayTracker.API.Models;

public class EmployeeInfraction
{
    public int EmployeeId { get; set; }
    public virtual Employee Employee { get; set; } = default!;
    public int InfractionId { get; set; }
    public virtual Infraction Infraction { get; set; } = default!;
    public int Year { get; set; }

    [Range(1, 12)]
    public int Month { get; set; }
    public int Count { get; set; }
}
