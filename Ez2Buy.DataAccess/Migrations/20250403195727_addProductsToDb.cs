using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Ez2Buy.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class addProductsToDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Categories",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ListPrice = table.Column<double>(type: "float", nullable: false),
                    Price = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "Description", "ListPrice", "Name", "Price" },
                values: new object[,]
                {
                    { 1, "The latest iPhone model with cutting-edge technology, featuring a powerful A15 Bionic chip, a stunning 6.1-inch Super Retina XDR display, and a pro camera system.", 1199.99, "iPhone 14 Pro", 1099.99 },
                    { 2, "The Samsung Galaxy S23 offers a sleek design with powerful performance, featuring a 6.1-inch AMOLED display, and the latest Snapdragon chipset for speed and efficiency.", 999.99000000000001, "Samsung Galaxy S23", 899.99000000000001 },
                    { 3, "Adidas Ultraboost 22 running shoes combine exceptional comfort with innovative design, featuring responsive Boost cushioning and a supportive Primeknit upper for a snug fit.", 180.0, "Adidas Ultraboost 22", 159.99000000000001 },
                    { 4, "A soft and breathable cotton t-shirt that is perfect for everyday casual wear. Available in a variety of colors and fits for all sizes.", 20.0, "Basic Cotton T-shirt", 15.0 },
                    { 5, "A fast-heating electric kettle with an automatic shut-off feature for safety, ideal for boiling water quickly for tea, coffee, and other beverages.", 24.989999999999998, "Electric Kettle", 19.989999999999998 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Products");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Categories",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(30)",
                oldMaxLength: 30);
        }
    }
}
