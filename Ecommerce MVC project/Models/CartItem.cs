namespace Ecommerce_MVC_project.Models
{
    public class CartItem
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public string ImagePath { get; set; }

        public decimal Total => Quantity * Price;
    }
}