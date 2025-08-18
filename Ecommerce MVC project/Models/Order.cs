// في ملف جديد اسمه Order.cs داخل مجلد Models

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ecommerce_MVC_project.Models
{
    public class Order
    {
        public int Id { get; set; }

        [Required]
        public string OrderStatus { get; set; } 

        [Required]
        public DateTime OrderDate { get; set; } = DateTime.Now;

        [Required]
        public decimal TotalPrice { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }

        public List<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
    }
}