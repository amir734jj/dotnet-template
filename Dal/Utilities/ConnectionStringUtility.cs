using System;
using Npgsql;
using StackExchange.Redis;

namespace Dal.Utilities
{
    public static class ConnectionStringUtility
    {
        public static string ConnectionStringUrlToRedisResource(string connectionStringUrl)
        {
            var result = Uri.TryCreate(connectionStringUrl, UriKind.Absolute, out var uri);

            if (!result)
            {
                throw new ArgumentException("Failed to parse Redis connection string");
            }

            var userInfo = uri.UserInfo.Split(':');
            var configurationOptions = new ConfigurationOptions
            {
                EndPoints = { { uri.Host, uri.Port } },
                ClientName = userInfo[0],
                Password = userInfo[1],
                AbortOnConnectFail = false
            };

            return configurationOptions.ToString();
        }

        /// <summary>
        /// Converts connection string url to resource
        /// </summary>
        /// <param name="connectionStringUrl"></param>
        /// <returns></returns>
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
                TrustServerCertificate = true,
                Pooling = true,
                // Hard limit
                MaxPoolSize = 5
            };

            return connectionStringBuilder.ToString();
        }
    }
}