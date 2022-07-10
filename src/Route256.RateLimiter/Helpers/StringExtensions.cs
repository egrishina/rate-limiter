using System.Text.RegularExpressions;

namespace Route256.RateLimiter.Helpers;

public static class StringExtensions
{
    public static bool IsUrlMatch(this string source, string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return false;
        }
        
        // if the regex is e.g. /api/values/ the path should be an exact match
        // if all paths below this should be included the regex should be /api/values/*
        if (value[^1] != '$')
        {
            value += '$';
        }
        
        if (value[0] != '^')
        {
            value = '^' + value;
        }
        
        return Regex.IsMatch(source, value, RegexOptions.IgnoreCase);
    }
    
    public static TimeSpan ToTimeSpan(this string timeSpan)
    {
        var l = timeSpan.Length - 1;
        var value = timeSpan.Substring(0, l);
        var type = timeSpan.Substring(l, 1);

        return type switch
        {
            "d" => TimeSpan.FromDays(double.Parse(value)),
            "h" => TimeSpan.FromHours(double.Parse(value)),
            "m" => TimeSpan.FromMinutes(double.Parse(value)),
            "s" => TimeSpan.FromSeconds(double.Parse(value)),
            _ => throw new FormatException($"{timeSpan} can't be converted to TimeSpan, unknown type {type}"),
        };
    }
}