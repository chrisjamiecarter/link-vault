using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LinkVault.Core.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "linkvault");

            migrationBuilder.CreateTable(
                name: "Links",
                schema: "linkvault",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OriginalUrl = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: false),
                    ShortCode = table.Column<string>(type: "varchar(16)", unicode: false, maxLength: 16, nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    ExpiresAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    QrCodeUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Sequence = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Links", x => x.Id);
                    table.UniqueConstraint("AK_Links_Sequence", x => x.Sequence);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Links_ExpiresAt",
                schema: "linkvault",
                table: "Links",
                column: "ExpiresAt");

            migrationBuilder.CreateIndex(
                name: "IX_Links_IsActive",
                schema: "linkvault",
                table: "Links",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_Links_ShortCode",
                schema: "linkvault",
                table: "Links",
                column: "ShortCode",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Links",
                schema: "linkvault");
        }
    }
}
