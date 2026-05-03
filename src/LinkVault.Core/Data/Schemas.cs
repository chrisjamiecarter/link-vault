namespace LinkVault.Core.Data;

internal static class Schemas
{
    internal sealed record SchemaMetadata(string Schema, string Table);

    internal static class LinkVault
    {
        private const string Name = "linkvault";

        public static readonly SchemaMetadata Links = new(Name, "Links");
    }

    internal static class Dbo
    {
        private const string Name = "dbo";

        public static readonly SchemaMetadata MigrationsHistory = new(Name, "__EFMigrationsHistory");
    }
}
