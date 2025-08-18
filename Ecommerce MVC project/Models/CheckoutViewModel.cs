using System.Collections.Generic;

namespace Ecommerce_MVC_project.Models
{
    public class CheckoutViewModel
    {
        public List<CartItem> CartItems { get; set; }
        public User CurrentUser { get; set; }
        public decimal CartTotal { get; set; }
    }
}