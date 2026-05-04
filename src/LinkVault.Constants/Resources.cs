namespace LinkVault.Constants;

public static class Resources
{
    private const string Project = "linkvault";

    public static class Cache
    {
        public const string Name = $"{Project}-cache";

        public const string DataVolume = $"{Project}-cache-data";
        public const string RedisInsight = $"{Project}-cache-redisinsight";
    }

    public static class Database
    {
        public const string Name = $"{Project}-database";
    }

    public static class DatabaseMigrator
    {
        public const string Name = $"{Project}-migrator";
    }

    public static class SqlServer
    {
        public const string Name = $"{Project}-sqlserver";

        public const string DataVolume = $"{Project}-sqlserver-data";
        public const string Endpoint = $"{Project}-sqlserver-endpoint";
        public const int Port = 14330;
    }

    public static class  WebApi
    {
        public const string Name = $"{Project}-api";
    }

    public static class WebBlazor
    {
        public const string Name = $"{Project}-blazor";
    }
}
