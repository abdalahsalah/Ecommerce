using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ecommerce_MVC_project.Models
{
    public class Product
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public decimal Price { get; set; }
        [Required]
        public int Stock { get; set; }
        public string ImagePath { get; set; }
        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public string Weight { get; set; }
        public string Size { get; set; }
        public string Color { get; set; }
        public int? UserId { get; set; } 
        public int? BrandId { get; set; } 
        public int? CategoryId { get; set; } 
        public User User { get; set; } 
        

        public Brand Brand { get; set; } 
        

        public Category Category { get; set; } 
    }
}
