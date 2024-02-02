using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HUECL.alpha._6_0.Migrations
{
    /// <inheritdoc />
    public partial class _20230723SaleDelivery : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SaleDeliveries",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Number = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DeliveryDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TotalDelivery = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    TotalNet = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    TotalTax = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    Comment = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Active = table.Column<int>(type: "int", nullable: false),
                    SaleId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SaleDeliveries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SaleDeliveries_Sales_SaleId",
                        column: x => x.SaleId,
                        principalTable: "Sales",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SaleDeliveryItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    Active = table.Column<int>(type: "int", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SaleDeliveryId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SaleDeliveryItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SaleDeliveryItems_SaleDeliveries_SaleDeliveryId",
                        column: x => x.SaleDeliveryId,
                        principalTable: "SaleDeliveries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SaleDeliveries_SaleId",
                table: "SaleDeliveries",
                column: "SaleId");

            migrationBuilder.CreateIndex(
                name: "IX_SaleDeliveryItems_SaleDeliveryId",
                table: "SaleDeliveryItems",
                column: "SaleDeliveryId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SaleDeliveryItems");

            migrationBuilder.DropTable(
                name: "SaleDeliveries");
        }
    }
}
