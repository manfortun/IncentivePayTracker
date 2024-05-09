using HttpConnection.Interfaces;

namespace IncentivePayTracker.Blazor.Services.Endpoints;

public class InfractionEndpoint : IEndpoint
{
    public string Name => "Infractions";

    public string Endpoint { get; init; }

    public InfractionEndpoint(IConfiguration config)
    {
        Endpoint = config.GetConnectionString(this.Name) ?? throw new NotImplementedException($"There is no connection string set for {Name}");
    }
}
