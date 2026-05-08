using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sweatbox.SharedKernel;

namespace LinkVault.Core.Database.Configurations;

internal static class EntityTypeBuilderExtensions
{
    extension<T>(EntityTypeBuilder<T> builder) where T : class
    {
        public EntityTypeBuilder<T> ToSchemaTable(SchemaTable schemaTable)
            => builder.ToTable(schemaTable.Table, schemaTable.Schema.Value);
    }
}
