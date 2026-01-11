using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace recruitlab.server.Migrations
{
    /// <inheritdoc />
    public partial class Offerletteradded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "OfferLetters",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ApplicationId = table.Column<int>(type: "int", nullable: false),
                    JobType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Compensation = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    WorkLocation = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ReportingTo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OfferDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    JoiningDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FilePath = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OfferLetters", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OfferLetters_Applications_ApplicationId",
                        column: x => x.ApplicationId,
                        principalTable: "Applications",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2026, 1, 11, 14, 13, 7, 36, DateTimeKind.Utc).AddTicks(1910), "$2a$11$G9TnJyybF8ZG0.FadgUJheJBccAXBW.BrzXOY8U4.Dd/Kd0A7zuem" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2026, 1, 11, 14, 13, 7, 36, DateTimeKind.Utc).AddTicks(1925), "$2a$11$G9TnJyybF8ZG0.FadgUJheJBccAXBW.BrzXOY8U4.Dd/Kd0A7zuem" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2026, 1, 11, 14, 13, 7, 36, DateTimeKind.Utc).AddTicks(1926), "$2a$11$G9TnJyybF8ZG0.FadgUJheJBccAXBW.BrzXOY8U4.Dd/Kd0A7zuem" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2026, 1, 11, 14, 13, 7, 36, DateTimeKind.Utc).AddTicks(1928), "$2a$11$G9TnJyybF8ZG0.FadgUJheJBccAXBW.BrzXOY8U4.Dd/Kd0A7zuem" });

            migrationBuilder.CreateIndex(
                name: "IX_OfferLetters_ApplicationId",
                table: "OfferLetters",
                column: "ApplicationId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OfferLetters");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2025, 12, 6, 13, 24, 12, 504, DateTimeKind.Utc).AddTicks(5548), "$2a$11$FcSsNjeSvHpTbf3hmM/O1.H61xbPNrNfF0oFI1iLLNM/Sw6HjF5gu" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2025, 12, 6, 13, 24, 12, 504, DateTimeKind.Utc).AddTicks(5580), "$2a$11$FcSsNjeSvHpTbf3hmM/O1.H61xbPNrNfF0oFI1iLLNM/Sw6HjF5gu" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2025, 12, 6, 13, 24, 12, 504, DateTimeKind.Utc).AddTicks(5582), "$2a$11$FcSsNjeSvHpTbf3hmM/O1.H61xbPNrNfF0oFI1iLLNM/Sw6HjF5gu" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2025, 12, 6, 13, 24, 12, 504, DateTimeKind.Utc).AddTicks(5584), "$2a$11$FcSsNjeSvHpTbf3hmM/O1.H61xbPNrNfF0oFI1iLLNM/Sw6HjF5gu" });
        }
    }
}
