using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HUECL.alpha._6_0.Migrations
{
    /// <inheritdoc />
    public partial class _20250130ProjectsUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Active",
                table: "Projects",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreationDate",
                table: "Projects",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "CurrencyId",
                table: "Projects",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Identifier",
                table: "Projects",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "LastUpdate",
                table: "Projects",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "Priority",
                table: "Projects",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "Probaility",
                table: "Projects",
                type: "decimal(18,4)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "ProjectSectorId",
                table: "Projects",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "Total",
                table: "Projects",
                type: "decimal(18,4)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateTable(
                name: "ProjectSectors",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Active = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectSectors", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Projects_CurrencyId",
                table: "Projects",
                column: "CurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_Projects_ProjectSectorId",
                table: "Projects",
                column: "ProjectSectorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Projects_Currencies_CurrencyId",
                table: "Projects",
                column: "CurrencyId",
                principalTable: "Currencies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Projects_ProjectSectors_ProjectSectorId",
                table: "Projects",
                column: "ProjectSectorId",
                principalTable: "ProjectSectors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Projects_Currencies_CurrencyId",
                table: "Projects");

            migrationBuilder.DropForeignKey(
                name: "FK_Projects_ProjectSectors_ProjectSectorId",
                table: "Projects");

            migrationBuilder.DropTable(
                name: "ProjectSectors");

            migrationBuilder.DropIndex(
                name: "IX_Projects_CurrencyId",
                table: "Projects");

            migrationBuilder.DropIndex(
                name: "IX_Projects_ProjectSectorId",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "Active",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "CreationDate",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "CurrencyId",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "Identifier",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "LastUpdate",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "Priority",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "Probaility",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "ProjectSectorId",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "Total",
                table: "Projects");
        }
    }
}
