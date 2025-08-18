using Ecommerce_MVC_project.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace Ecommerce_MVC_project.ViewComponents
{
    public class CartIconViewComponent : ViewComponent
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        const string CART_KEY = "shopping_cart";

        public CartIconViewComponent(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public IViewComponentResult Invoke()
        {
            var session = _httpContextAccessor.HttpContext.Session;
            var sessionCart = session.GetString(CART_KEY);
            int itemCount = 0;

            if (sessionCart != null)
            {
                var cart = JsonConvert.DeserializeObject<List<CartItem>>(sessionCart);
                itemCount = cart.Sum(item => item.Quantity);
            }

            return View(itemCount);
        }
    }
}