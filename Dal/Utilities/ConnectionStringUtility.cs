using System;
using Npgsql;

namespace Dal.Utilities;

public static class ConnectionStringUtility
{
    public static string ConnectionStringUrlToPgResource(string connectionStringUrl)
    {
        var result = Uri.TryCreate(connectionStringUrl, UriKind.Absolute, out var uri);

        if (!result)
        {
            throw new ArgumentException("Failed to parse postgres connection string");
        }

        var userInfo = uri.UserInfo.Split(':');
        var connectionStringBuilder = new NpgsqlConnectionStringBuilder
        {
            Host = uri.Host,
            Username = userInfo[0],
            Password = userInfo[1],
            Database = uri.Segments[1],
            ApplicationName = Environment.MachineName,
            SslMode = SslMode.Require,
            Pooling = true,
            // Hard limit
            MaxPoolSize = 5
        };

        return connectionStringBuilder.ToString();
    }
}