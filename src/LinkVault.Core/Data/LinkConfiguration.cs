using LinkVault.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LinkVault.Core.Data;

internal sealed class LinkConfiguration
    : IEntityTypeConfiguration<Link>
{
    public void Configure(EntityTypeBuilder<Link> builder)
    {
        builder.ToTable(Schemas.LinkVault.Links.Table, Schemas.LinkVault.Links.Schema);

        builder.HasKey(link => link.Id);

        builder
            .Property(link => link.Id)
            .HasColumnName(nameof(Link.Id))
            .IsRequired()
            .ValueGeneratedNever();

        builder.HasAlternateKey(link => link.Sequence);

        builder
            .Property(link => link.Sequence)
            .HasColumnName(nameof(Link.Sequence))
            .IsRequired()
            .ValueGeneratedOnAdd();

        builder
            .Property(link => link.OriginalUrl)
            .HasColumnName(nameof(Link.OriginalUrl))
            .HasMaxLength(Link.OriginalUrlMaxLength)
            .IsRequired();

        builder
            .HasIndex(link => link.ShortCode)
            .IsUnique();

        builder
            .Property(link => link.ShortCode)
            .HasColumnName(nameof(Link.ShortCode))
            .HasMaxLength(Link.ShortCodeMaxLength)
            .IsRequired()
            .IsUnicode(false);

        builder
            .Property(link => link.CreatedAt)
            .HasColumnName(nameof(Link.CreatedAt))
            .IsRequired();

        builder.HasIndex(link => link.ExpiresAt);

        builder
            .Property(link => link.ExpiresAt)
            .HasColumnName(nameof(Link.ExpiresAt));

        builder.HasIndex(link => link.IsActive);

        builder
            .Property(link => link.IsActive)
            .HasColumnName(nameof(Link.IsActive))
            .HasDefaultValue(true)
            .IsRequired();

        builder
            .Property(link => link.QrCodeUrl)
            .HasColumnName(nameof(Link.QrCodeUrl));
    }
}
