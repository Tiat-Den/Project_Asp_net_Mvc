using System; // Thư viện cơ bản
using System.Collections.Generic; // Thư viện danh sách
using System.ComponentModel.DataAnnotations; // Thư viện ràng buộc dữ liệu
using System.ComponentModel.DataAnnotations.Schema; // Thư viện cấu hình SQL
using Microsoft.AspNetCore.Identity; // Gọi thư viện để sử dụng IdentityUser

namespace Bai_Cuoi_Ky.Models // Khai báo không gian tên
{
    public class Order // Khai báo class Order
    {
        [Key] // Đánh dấu Khóa chính
        public int Id { get; set; } // Cột Id tự tăng

        [Display(Name = "Ngày đặt hàng")] // Nhãn hiển thị
        public DateTime OrderDate { get; set; } = DateTime.Now; // Cột lưu ngày đặt, mặc định là ngày hiện tại

        [Required] // Bắt buộc nhập
        [Display(Name = "Họ tên khách hàng")] // Nhãn hiển thị
        public string CustomerName { get; set; } // Cột lưu tên khách

        [Required] // Bắt buộc nhập
        [EmailAddress] // Ràng buộc định dạng Email
        public string Email { get; set; } // Cột lưu Email

        [Required] // Bắt buộc nhập
        [Display(Name = "Số điện thoại")] // Nhãn hiển thị
        public string PhoneNumber { get; set; } // Cột lưu số điện thoại

        [Required] // Bắt buộc nhập
        [Display(Name = "Địa chỉ nhận hàng")] // Nhãn hiển thị
        public string Address { get; set; } // Cột lưu địa chỉ

        [Display(Name = "Tổng tiền")] // Nhãn hiển thị
        [Column(TypeName = "decimal(18,2)")] // Ép kiểu dữ liệu tiền tệ trong SQL
        public decimal TotalAmount { get; set; } // Cột lưu tổng tiền

        [Display(Name = "Trạng thái")] // Nhãn hiển thị
        public string Status { get; set; } = "Pending"; // Cột trạng thái đơn, mặc định là Pending

        public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>(); // Danh sách chi tiết đơn hàng

        [Display(Name = "Tài khoản khách hàng")] // Nhãn hiển thị
        public string? UserId { get; set; } // Đổi kiểu dữ liệu sang string (chuỗi) để khớp với Id của IdentityUser

        [ForeignKey("UserId")] // Định nghĩa UserId là Khóa ngoại
        public virtual IdentityUser User { get; set; } // Liên kết tới bảng AspNetUsers mặc định của hệ thống
    }
}