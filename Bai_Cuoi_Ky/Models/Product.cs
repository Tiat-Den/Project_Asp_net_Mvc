using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bai_Cuoi_Ky.Models
{
    public class Product
    {
        [Key]

        public int Id { get; set; }

        [Required(ErrorMessage = "Tên sản phẩm không được để trống")]
        [StringLength(200)]
        [Display(Name = "Tên sản phẩm")]
        public string Name { get; set; }

        [Display(Name = "Mô tả")]
        public string Description { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Giá")]
        public decimal Price { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Giá khuyến mãi")]
        public decimal? DiscountPrice { get; set; }

        [Display(Name = "Hình ảnh")]
        public string ImageUrl { get; set; }
        public string? ImageUrl2 { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn danh mục")] 
        [Display(Name = "Danh mục")] 
        public int CategoryId { get; set; } 

        [ForeignKey("CategoryId")] 
        public virtual Category Category { get; set; } 

        [Display(Name = "Chất liệu")]
        public string Material { get; set; }
        public string? Gender { get; set; }

        [Display(Name = "Tồn kho")]
        public int Stock { get; set; }

        [Display(Name = "Còn hàng")]
        public bool IsAvailable { get; set; } = true;
        public bool IsFavorite { get; set; } = false;
        public bool IsNewArrival { get; set; } = false;

    }
}