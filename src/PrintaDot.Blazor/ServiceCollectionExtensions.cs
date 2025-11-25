using Microsoft.Extensions.DependencyInjection;

namespace PrintaDot.Blazor;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddPrintaDotClient(this IServiceCollection services)
    {
        services.AddScoped<IPrintaDotClient, PrintaDotClient>();
        return services;
    }
}
