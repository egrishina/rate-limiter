using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Route256.RateLimiter.Middleware;
using Route256.RateLimiter.Services;
using Route256.RateLimiter.Storage;
using Route256.RateLimiter.Storage.MemoryCache;

namespace Route256.RateLimiter;

public static class StartupExtensions
{
    public static IServiceCollection AddInMemoryRateLimiting(this IServiceCollection services)
    {
        services.AddSingleton<IClientStatisticsStorage, MemoryCacheClientStatisticsStorage>();
        services.AddSingleton<IRateLimitProcessor, RateLimitProcessor>();
        return services;
    }
    
    public static IApplicationBuilder UseIpRateLimiting(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<RateLimitMiddleware>();
    }
}