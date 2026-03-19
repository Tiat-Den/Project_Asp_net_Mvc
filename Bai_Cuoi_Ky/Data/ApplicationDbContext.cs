using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Bai_Cuoi_Ky.Models;

namespace Bai_Cuoi_Ky.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Vòng - Lắc" },
                new Category { Id = 2, Name = "Nhẫn" },
                new Category { Id = 3, Name = "Dây Chuyền" },
                new Category { Id = 4, Name = "Bông Tai" },
                new Category { Id = 5, Name = "Trang Sức Đôi" },
                new Category { Id = 6, Name = "Trang Sức Bộ" },
                new Category { Id = 7, Name = "Phụ Kiện" }
            );

            modelBuilder.Entity<Product>().HasData(
                new Product
                {
                    Id = 1,
                    Name = "Lắc tay bạc nữ Tennis chuỗi đá CZ dạng dây rút thắt nơ LILI_869163",
                    Description = "Đang cập nhật",
                    Price = 2578000,
                    DiscountPrice = 3000000,
                    CategoryId = 1,
                    Material = "Bạc",
                    Stock = 10,
                    IsAvailable = true,
                    ImageUrl = "/images/Products/Lac-tay-bac-nu-chuoi-da-CZ-dang-day-rut-that-no1.jpg",
                    ImageUrl2 = "/images/Products/Lac-tay-bac-nu-chuoi-da-CZ-dang-day-rut-that-no2.jpg"
                },
                new Product
                {
                    Id = 2,
                    Name = "Nhẫn bạc nữ đính kim cương Moissanite Aidan LILI_335168",
                    Description = "Đang cập nhật",
                    Price = 2181000,
                    DiscountPrice = 2500000,
                    CategoryId = 2,
                    Material = "Bạc",
                    Stock = 5,
                    IsAvailable = true,
                    ImageUrl = "/images/Products/Nhan-bac-nu-dinh-kim-cuong-Moissanite-Aidan1.jpg",
                    ImageUrl2 = "/images/Products/Nhan-bac-nu-dinh-kim-cuong-Moissanite-Aidan2.jpg"
                },
                new Product
                {
                    Id = 3,
                    Name = "Dây chuyền đôi bạc đính đá CZ hình cá voi và bướm Brenna LILI_123985",
                    Description = "Đang cập nhật",
                    Price = 520000,
                    DiscountPrice = 2000000,
                    CategoryId = 3,
                    Material = "Bạc",
                    Stock = 8,
                    IsAvailable = true,
                    ImageUrl = "/images/Products/Day-chuyen-doi-bac-dinh-da-CZ-hinh-ca-voi-va-buom1.jpg",
                    ImageUrl2 = "/images/Products/Day-chuyen-doi-bac-dinh-da-CZ-hinh-ca-voi-va-buom2.jpg"
                },
                new Product
                {
                    Id = 4,
                    Name = "Bông tai bạc nữ đính đá CZ hình chiếc nơ sang trọng LILI_698154",
                    Description = "Đang cập nhật",
                    Price = 1918000,
                    CategoryId = 4,
                    Material = "Bạc",
                    Stock = 15,
                    IsAvailable = true,
                    ImageUrl = "/images/Products/Bong-tai-bac-nu-hinh-that-no-dinh-da1.jpg",
                    ImageUrl2 = "/images/Products/Bong-tai-bac-nu-hinh-that-no-dinh-da2.jpg"
                },
                new Product
                {
                    Id = 5,
                    Name = "Lắc tay bạc cặp đôi tình yêu Forever Love LILI_986852",
                    Description = "Đang cập nhật",
                    Price = 3458000,
                    CategoryId = 5,
                    Material = "Bạc",
                    Stock = 15,
                    IsAvailable = true,
                    ImageUrl = "/images/Products/Lac-tay-bac-cap-doi-tinh-yeu-Forever-Love1.jpg",
                    ImageUrl2 = "/images/Products/Lac-tay-bac-cap-doi-tinh-yeu-Forever-Love2.jpg"
                },
                new Product
                {
                    Id = 6,
                    Name = "Bộ trang sức bạc nữ đính đá Garnet, CZ hoa hồng tình yêu LILI_727966",
                    Description = "Đang cập nhật",
                    Price = 1408000,
                    CategoryId = 6,
                    Material = "Bạc",
                    Stock = 15,
                    IsAvailable = true,
                    ImageUrl = "/images/Products/Bo-trang-suc-bac-nu-dinh-da-Garnet-CZ-hoa-hong-tinh-yeu1.jpg",
                    ImageUrl2 = "/images/Products/Bo-trang-suc-bac-nu-dinh-da-Garnet-CZ-hoa-hong-tinh-yeu2.jpg"
                },
                new Product
                {
                    Id = 7,
                    Name = "Tủ hộp đựng đồ trang sức phụ kiện có khóa gỗ óc chó Holly LILI_161276",
                    Description = "Đang cập nhật",
                    Price = 1809000,
                    CategoryId = 7,
                    Material = "",
                    Stock = 15,
                    IsAvailable = true,
                    ImageUrl = "/images/Products/Tu-hop-dung-do-trang-suc-phu-kien-dep-go-oc-cho1.jpg",
                    ImageUrl2 = "/images/Products/Tu-hop-dung-do-trang-suc-phu-kien-dep-go-oc-cho2.jpg"
                }
            );
        }
    }
}