using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace Bai_Cuoi_Ky.Models
{
    public class Order
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "Ngày đặt hàng")]
        public DateTime OrderDate { get; set; } = DateTime.Now;

        [Required(ErrorMessage = "Vui lòng nhập họ tên")]
        [Display(Name = "Họ tên người nhận")]
        public string CustomerName { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập email")]
        [EmailAddress(ErrorMessage = "Email không đúng định dạng")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập số điện thoại")]
        [Display(Name = "Số điện thoại")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập địa chỉ")]
        [Display(Name = "Địa chỉ nhận hàng")]
        public string Address { get; set; }

        // MỚI: Cột lưu phương thức thanh toán (COD hoặc QR)
        [Required]
        [Display(Name = "Phương thức thanh toán")]
        public string PaymentMethod { get; set; }

        [Display(Name = "Tổng tiền")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalAmount { get; set; }

        [Display(Name = "Trạng thái đơn hàng")]
        public string Status { get; set; } = "Pending"; // Pending, Confirmed, Shipping, Success, Cancelled

        [Display(Name = "Ghi chú")]
        public string? Notes { get; set; } // Để dấu ? vì ghi chú có thể để trống (null)

        // Liên kết 1-nhiều với OrderDetail
        public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();

        // Liên kết với IdentityUser (Tài khoản người dùng)
        public string? UserId { get; set; }
        [ForeignKey("UserId")]
        public virtual IdentityUser? User { get; set; }
    }
}