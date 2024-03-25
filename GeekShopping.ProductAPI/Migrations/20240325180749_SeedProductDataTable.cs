using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GeekShopping.ProductAPI.Migrations
{
    public partial class SeedProductDataTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "product",
                columns: new[] { "id", "category_name", "description", "image_url", "name", "price" },
                values: new object[,]
                {
                    { 3L, "T-Shirt", "Lorem ipsum enim commodo habitant dapibus lobortis ac, mollis viverra lobortis donec risus elementum, laoreet aenean quisque tincidunt eget augue.", "https://github.com/leandrocgsi/erudio-microservices-dotnet6/blob/main/ShoppingImages/2_no_internet.jpg", "Camisets Elon Musk", 69.9m },
                    { 4L, "T-Shirt", "Lorem ipsum enim commodo habitant dapibus lobortis ac, mollis viverra lobortis donec risus elementum, laoreet aenean quisque tincidunt eget augue.", "https://github.com/leandrocgsi/erudio-microservices-dotnet6/blob/main/ShoppingImages/2_no_internet.jpg", "Camiseta GNU Linux", 69.9m },
                    { 5L, "T-Shirt", "Lorem ipsum enim commodo habitant dapibus lobortis ac, mollis viverra lobortis donec risus elementum, laoreet aenean quisque tincidunt eget augue.", "https://github.com/leandrocgsi/erudio-microservices-dotnet6/blob/main/ShoppingImages/2_no_internet.jpg", "Camiseta Goku Fases", 69.9m },
                    { 6L, "T-Shirt", "Lorem ipsum enim commodo habitant dapibus lobortis ac, mollis viverra lobortis donec risus elementum, laoreet aenean quisque tincidunt eget augue.", "https://github.com/leandrocgsi/erudio-microservices-dotnet6/blob/main/ShoppingImages/2_no_internet.jpg", "Camiseta feminina Coffe Benefits", 69.9m },
                    { 7L, "Sweatshirt", "Lorem ipsum enim commodo habitant dapibus lobortis ac, mollis viverra lobortis donec risus elementum, laoreet aenean quisque tincidunt eget augue.", "https://github.com/leandrocgsi/erudio-microservices-dotnet6/blob/main/ShoppingImages/2_no_internet.jpg", "Moletom com Capuz Cobra Kai", 159.9m }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "product",
                keyColumn: "id",
                keyValue: 3L);

            migrationBuilder.DeleteData(
                table: "product",
                keyColumn: "id",
                keyValue: 4L);

            migrationBuilder.DeleteData(
                table: "product",
                keyColumn: "id",
                keyValue: 5L);

            migrationBuilder.DeleteData(
                table: "product",
                keyColumn: "id",
                keyValue: 6L);

            migrationBuilder.DeleteData(
                table: "product",
                keyColumn: "id",
                keyValue: 7L);
        }
    }
}
