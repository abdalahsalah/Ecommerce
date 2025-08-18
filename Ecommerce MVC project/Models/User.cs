using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ecommerce_MVC_project.Models
{
    public class User
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        [EmailAddress] 
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
        [Compare("Password",ErrorMessage ="Password Does Not Match")]
        [NotMapped]
        public string ConfirmPassword { get; set; }

        [Required]
        public string Role { get; set; } = "Customer";

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        [Required]
        public int Age { get; set; }

      
        public string? PhoneNumber { get; set; }
        public string? Address { get; set; }

        public List<Order> Orders { get; set; } = new List<Order>();
        public List<Product> Products { get; set; } = new List<Product>();
        public List<Brand> Brands { get; set; } = new List<Brand>();
        public List<Category> Categories { get; set; } = new List<Category>();
    }
}