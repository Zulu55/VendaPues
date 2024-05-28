using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Orders.Backend.Migrations
{
    /// <inheritdoc />
    public partial class AddStockToInventoryDetail : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<float>(
                name: "Stock",
                table: "InventoryDetails",
                type: "real",
                nullable: false,
                defaultValue: 0f);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Stock",
                table: "InventoryDetails");
        }
    }
}
