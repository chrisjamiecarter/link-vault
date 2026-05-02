namespace LinkVault.Constants;

public static class ResourceNames
{
    private const string Project = "linkvault";

    public static class SqlServer
    {
        public const string Name = $"{Project}-sqlserver";

        public const int Port = 1433;
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
