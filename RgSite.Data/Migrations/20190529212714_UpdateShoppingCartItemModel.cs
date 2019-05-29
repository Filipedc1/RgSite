using Microsoft.EntityFrameworkCore.Migrations;

namespace RgSite.Data.Migrations
{
    public partial class UpdateShoppingCartItemModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PriceId",
                table: "ShoppingCartItems",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PriceId",
                table: "ShoppingCartItems");
        }
    }
}
