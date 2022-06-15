using System.Net;
using Microsoft.AspNetCore.Http;
using Route256.RateLimiter.Core;
using Route256.RateLimiter.Helpers;
using Route256.RateLimiter.Models;

namespace Route256.RateLimiter.Middleware;

public class RateLimitMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IRateLimitProcessor _processor;

    public RateLimitMiddleware(RequestDelegate next, IRateLimitProcessor processor)
    {
        _next = next;
        _processor = processor;
    }

    public async Task Invoke(HttpContext context)
    {
        var identity = await ResolveIdentityAsync(context);
        var rules = await _processor.GetMatchingRulesAsync(identity, context.RequestAborted);
        foreach (var rule in rules)
        {
            var clientStatistics = await _processor.GetClientStatisticsAsync(identity, context.RequestAborted);
            if (clientStatistics.Timestamp.Add(rule.Period.ToTimeSpan()) >= DateTime.UtcNow && 
                clientStatistics.Count > rule.Limit)
            {
                context.Response.StatusCode = (int)HttpStatusCode.TooManyRequests;
                return;
            }
            
            await _processor.UpdateClientStatisticsAsync(identity, clientStatistics, context.RequestAborted);
        }
        
        await _next(context);
    }

    private async Task<ClientRequestIdentity> ResolveIdentityAsync(HttpContext httpContext)
    {
        var clientIp = httpContext.Connection.RemoteIpAddress?.ToString() ?? string.Empty;
        var path = httpContext.Request.Path.ToString().ToLowerInvariant();
        return new ClientRequestIdentity
        {
            ClientIp = clientIp,
            Path = path == "/" ? path : path.TrimEnd('/'),
            HttpVerb = httpContext.Request.Method.ToLowerInvariant()
        };
    }
}