namespace Bai_Cuoi_Ky.Models
{
    public class CartItem
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public string ImageUrl { get; set; }
        public string ImageUrl2 { get; set; }
        public decimal Total => Price * Quantity;
    }
}