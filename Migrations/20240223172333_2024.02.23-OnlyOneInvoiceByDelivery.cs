using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HUECL.alpha._6_0.Migrations
{
    /// <inheritdoc />
    public partial class _20240223OnlyOneInvoiceByDelivery : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_SaleInvoices_SaleDeliveryId",
                table: "SaleInvoices");

            migrationBuilder.CreateIndex(
                name: "IX_SaleInvoices_SaleDeliveryId",
                table: "SaleInvoices",
                column: "SaleDeliveryId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_SaleInvoices_SaleDeliveryId",
                table: "SaleInvoices");

            migrationBuilder.CreateIndex(
                name: "IX_SaleInvoices_SaleDeliveryId",
                table: "SaleInvoices",
                column: "SaleDeliveryId");
        }
    }
}
