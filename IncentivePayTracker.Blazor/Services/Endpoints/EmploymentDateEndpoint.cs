using HttpConnection.Interfaces;

namespace IncentivePayTracker.Blazor.Services.Endpoints;

public class EmploymentDateEndpoint : IEndpoint
{
    public string Name => "EmploymentDates";

    public string Endpoint { get; init; }

    public EmploymentDateEndpoint(IConfiguration config)
    {
        Endpoint = config.GetConnectionString(this.Name) ?? throw new NotImplementedException($"There is no connection string set for {Name}");
    }
}
