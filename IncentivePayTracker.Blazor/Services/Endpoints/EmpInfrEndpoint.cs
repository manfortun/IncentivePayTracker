using HttpConnection.Interfaces;

namespace IncentivePayTracker.Blazor.Services.Endpoints;

public class EmpInfrEndpoint : IEndpoint
{
    public string Name => "EmployeeInfractions";

    public string Endpoint { get; init; }

    public EmpInfrEndpoint(IConfiguration config)
    {
        Endpoint = config.GetConnectionString(this.Name) ?? throw new NotImplementedException($"There is no connection string set for {Name}");
    }
}
