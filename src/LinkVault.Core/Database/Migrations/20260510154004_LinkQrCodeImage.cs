using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LinkVault.Core.Database.Migrations;

/// <inheritdoc />
public partial class LinkQrCodeImage : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "QrCodeUrl",
            schema: "linkvault",
            table: "Links");

        migrationBuilder.AddColumn<byte[]>(
            name: "QrCodeImage",
            schema: "linkvault",
            table: "Links",
            type: "varbinary(max)",
            nullable: true);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "QrCodeImage",
            schema: "linkvault",
            table: "Links");

        migrationBuilder.AddColumn<string>(
            name: "QrCodeUrl",
            schema: "linkvault",
            table: "Links",
            type: "nvarchar(max)",
            nullable: true);
    }
}
