using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Middleware {
public static class Extensions {
    public static IServiceCollection AddManager(this  IServiceCollection services) {
        services.AddSingleton<Manager>();
        return services;
    }
}
}