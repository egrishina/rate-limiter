using Microsoft.Extensions.Options;
using Route256.RateLimiter.Helpers;
using Route256.RateLimiter.Models;
using Route256.RateLimiter.Options;
using Route256.RateLimiter.Storage;

namespace Route256.RateLimiter.Core;

public class RateLimitProcessor : IRateLimitProcessor
{
    private readonly RateLimitOptions _options;
    private readonly IRateLimitStorage _limitStorage;
    private readonly IClientStatisticsStorage _clientStorage;

    public RateLimitProcessor(IOptions<RateLimitOptions> options, IRateLimitStorage limitStorage,
        IClientStatisticsStorage clientStorage)
    {
        _options = options.Value;
        _limitStorage = limitStorage;
        _clientStorage = clientStorage;
    }

    public async Task<IEnumerable<RateLimitRule>> GetMatchingRulesAsync(ClientRequestIdentity identity,
        CancellationToken cancellationToken = default)
    {
        var policies = await _limitStorage.GetAsync(cancellationToken);
        var individualRules = policies.IpRules.SelectMany(r => r.Rules).ToList();
        var limits = new List<RateLimitRule>();

        if (individualRules.Any())
        {
            // search for rules with endpoints like "*" and "*:/matching_path"
            var path = $".+:{identity.Path}";
            var pathLimits = individualRules.Where(r => path.IsUrlMatch(r.Endpoint));
            limits.AddRange(pathLimits);

            // search for rules with endpoints like "matching_verb:/matching_path"
            var verbLimits = individualRules.Where(r =>
                $"{identity.HttpVerb}:{identity.Path}".IsUrlMatch(r.Endpoint));
            limits.AddRange(verbLimits);

            // get the most restrictive limit for each period 
            limits = limits.GroupBy(l => l.Period).Select(l => l.OrderBy(x => x.Limit)).Select(l => l.First()).ToList();
        }

        // search for matching general rules
        if (_options.GeneralRules.Any())
        {
            var matchingGeneralLimits = new List<RateLimitRule>();
            // search for rules with endpoints like "*" and "*:/matching_path" in general rules
            var pathLimits = _options.GeneralRules.Where(r =>
                $"*:{identity.Path}".IsUrlMatch(r.Endpoint));
            matchingGeneralLimits.AddRange(pathLimits);

            // search for rules with endpoints like "matching_verb:/matching_path" in general rules
            var verbLimits = _options.GeneralRules.Where(r =>
                $"{identity.HttpVerb}:{identity.Path}".IsUrlMatch(r.Endpoint));
            matchingGeneralLimits.AddRange(verbLimits);

            // get the most restrictive general limit for each period 
            var generalLimits = matchingGeneralLimits
                .GroupBy(l => l.Period)
                .Select(l => l.OrderBy(x => x.Limit).ThenBy(x => x.Endpoint))
                .Select(l => l.First())
                .ToList();

            foreach (var generalLimit in generalLimits)
            {
                // add general rule if no specific rule is declared for the specified period
                if (!limits.Exists(l => l.Period == generalLimit.Period))
                {
                    limits.Add(generalLimit);
                }
            }
        }

        return limits;
    }

    public async Task<ClientStatistics> GetClientStatisticsAsync(ClientRequestIdentity identity,
        CancellationToken cancellationToken = default)
    {
        var key = $"{identity.Path}_{identity.ClientIp}";
        return await _clientStorage.GetValueAsync(identity.ClientIp);
    }

    public async Task UpdateClientStatisticsAsync(ClientRequestIdentity identity, ClientStatistics clientStatistics,
        CancellationToken cancellationToken = default)
    {
        var key = $"{identity.Path}_{identity.ClientIp}";
        await _clientStorage.UpdateValueAsync(identity.ClientIp, clientStatistics);
    }
}