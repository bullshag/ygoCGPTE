namespace WinFormsApp2
{
    internal static class DatabaseConfig
    {
        private const string Host = "76.134.86.9";
        private const string KimHost = "10.0.0.30";
        private const string Username = "userclient";
        private const string Password = "123321";
        public static bool DebugMode { get; set; }
        public static bool UseKimServer { get; set; }
        public static string ConnectionString =>
            $"Server={(UseKimServer ? KimHost : (DebugMode ? "127.0.0.1" : Host))};Database=accounts;User ID={Username};Password={Password};";
    }
}
