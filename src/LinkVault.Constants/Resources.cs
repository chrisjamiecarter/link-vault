namespace LinkVault.Constants;

public static class Resources
{
    private const string Project = "linkvault";

    public static class SqlServer
    {
        public const string Name = $"{Project}-sqlserver";

        public const string DataVolumne = $"{Project}-sqlserver-data";
        public const string Endpoint = $"{Project}-sqlserver-endpoint";
        public const int Port = 14330;
    }

    public static class Database
    {
        public const string Name = $"{Project}-db";
    }

    public static class  WebApi
    {
        public const string Name = $"{Project}-api";
    }

    public static class WebBlazor
    {
        public const string Name = $"{Project}-fe";
    }
}
