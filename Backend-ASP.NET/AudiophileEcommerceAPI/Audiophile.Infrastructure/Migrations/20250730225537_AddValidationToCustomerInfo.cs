using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AudiophileEcommerceAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddValidationToCustomerInfo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.AddColumn<bool>(
            //    name: "IsNew",
            //    table: "Products",
            //    type: "bit",
            //    nullable: true);

            //migrationBuilder.AddColumn<bool>(
            //    name: "IsPromotion",
            //    table: "Products",
            //    type: "bit",
            //    nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Password",
                table: "Customers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "CustomerInfoId1",
                table: "Carts",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Carts_CustomerInfoId1",
                table: "Carts",
                column: "CustomerInfoId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Carts_Customers_CustomerInfoId1",
                table: "Carts",
                column: "CustomerInfoId1",
                principalTable: "Customers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Carts_Customers_CustomerInfoId1",
                table: "Carts");

            migrationBuilder.DropIndex(
                name: "IX_Carts_CustomerInfoId1",
                table: "Carts");

            migrationBuilder.DropColumn(
                name: "IsNew",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "IsPromotion",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "Password",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "CustomerInfoId1",
                table: "Carts");
        }
    }
}
