using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Bai_Cuoi_Ky.Migrations
{
    /// <inheritdoc />
    public partial class SeedProductDataWithImages : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Products_Categories_CategoryId",
                table: "Products");

            migrationBuilder.DropTable(
                name: "Categories");

            migrationBuilder.DropIndex(
                name: "IX_Products_CategoryId",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "CategoryId",
                table: "Products");

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "Category", "Description", "DiscountPrice", "ImageUrl", "ImageUrl2", "IsAvailable", "Material", "Name", "Price", "Stock" },
                values: new object[,]
                {
                    { 1, "Vòng - Lắc", "Đang cập nhật", null, "/images/Products/Lac-tay-bac-nu-chuoi-da-CZ-dang-day-rut-that-no1.jpg", "/images/Products/Lac-tay-bac-nu-chuoi-da-CZ-dang-day-rut-that-no2.jpg", true, "Bạc", "Lắc tay bạc nữ Tennis chuỗi đá CZ dạng dây rút thắt nơ LILI_869163", 2578000m, 10 },
                    { 2, "Nhẫn", "Đang cập nhật", null, "/images/Products/Nhan-bac-nu-dinh-kim-cuong-Moissanite-Aidan1.jpg", "/images/Products/Nhan-bac-nu-dinh-kim-cuong-Moissanite-Aidan2.jpg", true, "Bạc", "Nhẫn bạc nữ đính kim cương Moissanite Aidan LILI_335168", 2181000m, 5 },
                    { 3, "Dây Chuyền", "Đang cập nhật", null, "/images/Products/Day-chuyen-doi-bac-dinh-da-CZ-hinh-ca-voi-va-buom1.jpg", "/images/Products/Day-chuyen-doi-bac-dinh-da-CZ-hinh-ca-voi-va-buom2.jpg", true, "Bạc", "Dây chuyền đôi bạc đính đá CZ hình cá voi và bướm Brenna LILI_123985", 520000m, 8 },
                    { 4, "Bông Tai", "Đang cập nhật", null, "/images/Products/Bong-tai-bac-nu-hinh-that-no-dinh-da1.jpg", "/images/Products/Bong-tai-bac-nu-hinh-that-no-dinh-da2.jpg", true, "Bạc", "Bông tai bạc nữ đính đá CZ hình chiếc nơ sang trọng LILI_698154", 1918000m, 15 },
                    { 5, "Trang Sức Đôi", "Đang cập nhật", null, "/images/Products/Lac-tay-bac-cap-doi-tinh-yeu-Forever-Love1.jpg", "/images/Products/Lac-tay-bac-cap-doi-tinh-yeu-Forever-Love2.jpg", true, "Bạc", "Lắc tay bạc cặp đôi tình yêu Forever Love LILI_986852", 3458000m, 15 },
                    { 6, "Trang Sức Bộ", "Đang cập nhật", null, "/images/Products/Bo-trang-suc-bac-nu-dinh-da-Garnet-CZ-hoa-hong-tinh-yeu1.jpg", "/images/Products/Bo-trang-suc-bac-nu-dinh-da-Garnet-CZ-hoa-hong-tinh-yeu2.jpg", true, "Bạc", "Bộ trang sức bạc nữ đính đá Garnet, CZ hoa hồng tình yêu LILI_727966", 1408000m, 15 },
                    { 7, "Phụ Kiện", "Đang cập nhật", null, "/images/Products/Tu-hop-dung-do-trang-suc-phu-kien-dep-go-oc-cho1.jpg", "/images/Products/Tu-hop-dung-do-trang-suc-phu-kien-dep-go-oc-cho2.jpg", true, "", "Tủ hộp đựng đồ trang sức phụ kiện có khóa gỗ óc chó Holly LILI_161276", 1809000m, 15 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.AddColumn<int>(
                name: "CategoryId",
                table: "Products",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Products_CategoryId",
                table: "Products",
                column: "CategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Products_Categories_CategoryId",
                table: "Products",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id");
        }
    }
}
