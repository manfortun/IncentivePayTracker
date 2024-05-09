namespace IncentivePayTracker.Blazor.ViewModels;

public class InfractionAdjustment
{
    public enum Type
    {
        Saved, Deduction, Addition
    }

    public int Id { get; set; }
    public Type InfractionType { get; set; }
}
