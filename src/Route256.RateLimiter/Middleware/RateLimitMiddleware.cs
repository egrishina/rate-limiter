using System.Net;
using Microsoft.AspNetCore.Http;
using Route256.RateLimiter.Helpers;
using Route256.RateLimiter.Models;
using Route256.RateLimiter.Options;
using Route256.RateLimiter.Services;

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
        var rules = (await _processor.GetMatchingRulesAsync(identity, context.RequestAborted)).ToList();

        var endpoint = context.GetEndpoint();
        var decorator = endpoint?.Metadata.GetMetadata<RateLimitRule>();
        if (decorator is not null && !rules.Exists(l => l.Period == decorator.Period))
        {
            rules.Add(decorator);
        }

        // get the most restrictive rule
        var limitRule = rules.First();
        foreach (var rule in rules)
        {
            if (rule.Limit / (rule.Period.ToTimeSpan() / TimeSpan.FromMilliseconds(1)) <
                limitRule.Limit / (limitRule.Period.ToTimeSpan() / TimeSpan.FromMilliseconds(1)))
            {
                limitRule = rule;
            }
        }

        var clientStatistics = await _processor.GetClientStatisticsAsync(identity, context.RequestAborted);
        if (clientStatistics.Timestamp.Add(limitRule.Period.ToTimeSpan()) >= DateTime.UtcNow &&
            clientStatistics.Count >= limitRule.Limit)
        {
            context.Response.StatusCode = (int)HttpStatusCode.TooManyRequests;
            return;
        }

        await _processor.UpdateClientStatisticsAsync(identity, clientStatistics, limitRule, context.RequestAborted);

        await _next(context);
    }

    private async Task<ClientRequestIdentity> ResolveIdentityAsync(HttpContext httpContext)
    {
        var clientIp = ResolveIp(httpContext);
        var path = httpContext.Request.Path.ToString().ToLowerInvariant();
        return new ClientRequestIdentity
        {
            ClientIp = clientIp,
            Path = path == "/" ? path : path.TrimEnd('/')
        };
    }

    private string ResolveIp(HttpContext httpContext)
    {
        var clientIp = httpContext.Connection.RemoteIpAddress;
        if (clientIp is null && httpContext.Request.Headers.TryGetValue("IP_address", out var values))
        {
            clientIp = IPAddress.Parse(values.Last());
        }

        return clientIp?.ToString() ?? string.Empty;
    }
}