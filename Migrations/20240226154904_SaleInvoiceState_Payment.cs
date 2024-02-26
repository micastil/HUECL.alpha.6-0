using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HUECL.alpha._6_0.Migrations
{
    /// <inheritdoc />
    public partial class SaleInvoiceState_Payment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "InvoiceState",
                table: "SaleInvoices",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "InvoiceState",
                table: "SaleInvoices");
        }
    }
}
