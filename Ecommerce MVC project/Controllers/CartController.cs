using Ecommerce_MVC_project.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ecommerce_MVC_project.Controllers
{
    public class CartController : Controller
    {
        private readonly EFContext db;
        const string CART_KEY = "shopping_cart";

        public CartController(EFContext context)
        {
            db = context;
        }

        private List<CartItem> GetCartItems()
        {
            var sessionCart = HttpContext.Session.GetString(CART_KEY);
            return sessionCart == null ? new List<CartItem>() : JsonConvert.DeserializeObject<List<CartItem>>(sessionCart);
        }

        private void SaveCartItems(List<CartItem> cart)
        {
            HttpContext.Session.SetString(CART_KEY, JsonConvert.SerializeObject(cart));
        }

        public IActionResult Index()
        {
            var cart = GetCartItems();
            return View(cart);
        }

        [HttpPost]
        public async Task<IActionResult> AddToCart(int productId, int quantity = 1)
        {
            var product = await db.Products.FindAsync(productId);
            if (product == null) { return NotFound(); }

            if (product.Stock < quantity)
            {
                TempData["ErrorMessage"] = $"Sorry, only {product.Stock} items of '{product.Name}' are available.";
                return RedirectToAction("Details", "Product", new { id = productId });
            }

            var cart = GetCartItems();
            var cartItem = cart.Find(p => p.ProductId == productId);

            if (cartItem != null)
            {
                cartItem.Quantity += quantity;
            }
            else
            {
                cart.Add(new CartItem
                {
                    ProductId = productId,
                    ProductName = product.Name,
                    Quantity = quantity,
                    Price = product.Price,
                    ImagePath = product.ImagePath
                });
            }

            SaveCartItems(cart);

            TempData["SuccessMessage"] = $"{product.Name} (x{quantity}) has been added to your cart.";

            string referer = Request.Headers["Referer"].ToString();
            if (!string.IsNullOrEmpty(referer))
            {
                return Redirect(referer);
            }
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public IActionResult UpdateCart(int productId, int quantity)
        {
            var cart = GetCartItems();
            var cartItem = cart.Find(p => p.ProductId == productId);

            if (cartItem != null)
            {


                if (quantity > 0)
                {
                    cartItem.Quantity = quantity;
                }
                else
                {
                    cart.Remove(cartItem);
                }
                SaveCartItems(cart);
            }
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult RemoveFromCart(int productId)
        {
            var cart = GetCartItems();
            var cartItem = cart.Find(p => p.ProductId == productId);
            if (cartItem != null)
            {
                cart.Remove(cartItem);
                SaveCartItems(cart);
            }
            return RedirectToAction("Index");
        }
    }
}