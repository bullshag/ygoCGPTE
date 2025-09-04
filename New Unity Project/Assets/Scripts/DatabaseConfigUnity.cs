using System;
using UnityEngine;

namespace UnityClient
{
    public static class DatabaseConfigUnity
    {
        private const string DatabaseName = "accounts";
        private static ServerConfig _config;

        private static ServerConfig Config =>
            _config ??= Resources.Load<ServerConfig>("ServerConfig");

        public static bool DebugMode { get; set; }
        public static bool UseKimServer { get; set; }

        private static string Username =>
            Environment.GetEnvironmentVariable("DB_USERNAME") ?? string.Empty;
        private static string Password =>
            Environment.GetEnvironmentVariable("DB_PASSWORD") ?? string.Empty;

        public static string ConnectionString =>
            $"Server={(UseKimServer ? Config.kimHost : (DebugMode ? "127.0.0.1" : Config.host))};" +
            $"Database={DatabaseName};User ID={Username};Password={Password};";

        public static string ApiBaseUrl => Config.apiBaseUrl;
    }
}
