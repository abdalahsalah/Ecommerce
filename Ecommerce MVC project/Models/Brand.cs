using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ecommerce_MVC_project.Models
{
    public class Brand
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
       
        public string ImagePath { get; set; }
        [Required]
        public string Description { get; set; }
        public int? UserId { get; set; } 

        public User User { get; set; } 
        public List<Product> Products { get; set; } = new List<Product>();
    }
}
