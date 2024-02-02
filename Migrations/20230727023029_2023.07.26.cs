using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HUECL.alpha._6_0.Migrations
{
    /// <inheritdoc />
    public partial class _20230726 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SaleItemId",
                table: "SaleDeliveryItems",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_SaleDeliveryItems_SaleItemId",
                table: "SaleDeliveryItems",
                column: "SaleItemId");

            migrationBuilder.AddForeignKey(
                name: "FK_SaleDeliveryItems_SaleItems_SaleItemId",
                table: "SaleDeliveryItems",
                column: "SaleItemId",
                principalTable: "SaleItems",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SaleDeliveryItems_SaleItems_SaleItemId",
                table: "SaleDeliveryItems");

            migrationBuilder.DropIndex(
                name: "IX_SaleDeliveryItems_SaleItemId",
                table: "SaleDeliveryItems");

            migrationBuilder.DropColumn(
                name: "SaleItemId",
                table: "SaleDeliveryItems");
        }
    }
}
