using IncentivePayTracker.DTO;

namespace IncentivePayTracker.Blazor.Services;

public static class InfractionExtensions
{
    public const double TOTAL_INCENTIVE = 5000.00d;
    private static Dictionary<int, string> _infractionClass = new Dictionary<int, string>()
    {
        {1, "type-a"},
        {2, "type-b"},
        {3, "type-c"},
        {4, "type-d"},
        {5, "type-e"},
        {6, "type-f"},
        {7, "type-g"},
        {8, "type-h"},
        {9, "type-i"},
    };
    public static Dictionary<int, string> InfractionClass => _infractionClass;

    public static double GetInfractionsTotal(this CompositeEmployeeInfraction empInfr)
    {
        double total = 0;

        if (empInfr.Infractions is not null)
        {
            foreach (var infraction in empInfr.Infractions)
            {
                total += infraction.Count * infraction.Infraction.Amount;
            }
        }

        return total;
    }

    public static double GetRemainingIncentive(this CompositeEmployeeInfraction empInfr)
    {
        double totalDeduction = empInfr.GetInfractionsTotal();
        double remaining = TOTAL_INCENTIVE - totalDeduction;

        return Math.Max(0, remaining);
    }

    public static int GetInfractionsCount(this CompositeEmployeeInfraction empInfr)
    {
        return empInfr.Infractions?.Sum(i => i.Count) ?? 0;
    }

    public static double GetDeductionPercentage(this CompositeEmployeeInfraction empInfr)
    {
        return Math.Min(TOTAL_INCENTIVE, empInfr.GetInfractionsTotal()) / TOTAL_INCENTIVE * 100;
    }

    public static double GetIncentivePercentage(this CompositeEmployeeInfraction empInfr)
    {
        return Math.Abs(empInfr.GetDeductionPercentage() - 100);
    }
}
