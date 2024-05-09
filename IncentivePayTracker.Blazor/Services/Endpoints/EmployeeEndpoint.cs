using HttpConnection.Interfaces;

namespace IncentivePayTracker.Blazor.Services.Endpoints;

public class EmployeeEndpoint : IEndpoint
{
    public string Name => "Employees";

    public string Endpoint { get; init; }

    public EmployeeEndpoint(IConfiguration config)
    {
        Endpoint = config.GetConnectionString(this.Name) ?? throw new NotImplementedException($"There is no connection string set for {Name}");
    }
}
