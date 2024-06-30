namespace Api.Extensions;

using System;
using Microsoft.Extensions.Configuration;

public static class ConfigurationExtension
{
    public static T GetRequiredValue<T>(this IConfiguration configuration, string key)
        where T : class
    {
        return configuration.GetValue<T>(key) ?? throw new Exception($"Failed to get configuration for: {key} and type: {typeof(T).Name}");
    }
}