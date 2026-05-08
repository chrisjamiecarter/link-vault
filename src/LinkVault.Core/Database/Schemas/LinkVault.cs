using Sweatbox.SharedKernel;

namespace LinkVault.Core.Database.Schemas;

internal static class LinkVault
{
    private static readonly SchemaName Schema = new("linkvault");

    public static readonly SchemaTable Links = Schema.Table("Links");
}
