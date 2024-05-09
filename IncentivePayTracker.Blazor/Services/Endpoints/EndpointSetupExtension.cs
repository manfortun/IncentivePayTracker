using HttpConnection;
using HttpConnection.Interfaces;
using System.Reflection;

namespace IncentivePayTracker.Blazor.Services.Endpoints;

public static class EndpointSetupExtension
{
    public static void HttpServiceStartup(this WebApplicationBuilder app)
    {
        Type abstractType = typeof(IEndpoint);
        Assembly assembly = Assembly.GetExecutingAssembly();

        var implementationsList = assembly.GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract && abstractType.IsAssignableFrom(t));

        foreach (var imp in implementationsList)
        {
            var methodInfo = typeof(HttpConnExtensions)
                .GetMethod(nameof(HttpConnExtensions.AddHttpEndpoint))?
                .MakeGenericMethod(imp);

            methodInfo?.Invoke(null, [app.Services, app.Configuration]);
        }
    }
}
