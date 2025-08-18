
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ecommerce_MVC_project.Models
{
    public class OrderDetail
    {
        public int Id { get; set; }

        [Required]
        public int Quantity { get; set; } 

        [Required]
        public decimal Price { get; set; } 

        public int OrderId { get; set; }
        public Order Order { get; set; }

        public int ProductId { get; set; }
        public Product Product { get; set; }
    }
}