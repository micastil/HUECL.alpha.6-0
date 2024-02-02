using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HUECL.alpha._6_0.Migrations
{
    /// <inheritdoc />
    public partial class _20231122DeliveryState : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DeliveryState",
                table: "SaleDeliveries",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeliveryState",
                table: "SaleDeliveries");
        }
    }
}
