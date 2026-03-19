using Bai_Cuoi_Ky.Models;
using System.ComponentModel.DataAnnotations;

public class Category
{
    [Key]
    public int Id { get; set; }

    [Required(ErrorMessage = "Tên danh mục không được để trống")]
    [StringLength(100)]
    [Display(Name = "Tên danh mục")]
    public string Name { get; set; }

    [Display(Name = "Mô tả")]
    public string? Description { get; set; }

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}