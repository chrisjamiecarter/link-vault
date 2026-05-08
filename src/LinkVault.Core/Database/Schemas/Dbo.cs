using Sweatbox.SharedKernel;

namespace LinkVault.Core.Database.Schemas;

internal static class Dbo
{
    private static readonly SchemaName SchemaName = new("dbo");

    public static readonly SchemaTable MigrationsHistory = SchemaName.Table("__EFMigrationsHistory");
}
