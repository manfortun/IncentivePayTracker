namespace IncentivePayTracker.DTO;

public class AnnualInfractionSummary
{
    public int InfractionId { get; set; }
    public double AmountPerInfraction { get; set; }
    public int Count { get; set; }
    public int Year { get; set; }
    public int Month { get; set; }
}
