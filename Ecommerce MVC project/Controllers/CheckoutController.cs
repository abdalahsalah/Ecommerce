using Ecommerce_MVC_project.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Ecommerce_MVC_project.Controllers
{
    
    [Authorize]
    public class CheckoutController : Controller
    {
        private readonly EFContext db;
        const string CART_KEY = "shopping_cart";

        public CheckoutController(EFContext context)
        {
            db = context;
        }

        private List<CartItem> GetCartItems()
        {
            var sessionCart = HttpContext.Session.GetString(CART_KEY);
            return sessionCart == null ? new List<CartItem>() : JsonConvert.DeserializeObject<List<CartItem>>(sessionCart);
        }

        public async Task<IActionResult> Index()
        {
            var cart = GetCartItems();
            if (!cart.Any())
            {
                TempData["ErrorMessage"] = "Your cart is empty. Please add items to your cart first.";
                return RedirectToAction("Index", "Cart");
            }

            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdString, out var userId))
            {
                return Challenge(); 
            }

            var user = await db.Users.FindAsync(userId);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            var viewModel = new CheckoutViewModel
            {
                CartItems = cart,
                CurrentUser = user,
                CartTotal = cart.Sum(item => item.Total)
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PlaceOrder(CheckoutViewModel model)
        {
            var cart = GetCartItems();
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userIdString) || !cart.Any())
            {
                return RedirectToAction("Index", "Home");
            }

            var userId = int.Parse(userIdString);

            foreach (var cartItem in cart)
            {
                var productInStock = await db.Products.FindAsync(cartItem.ProductId);
                if (productInStock == null || productInStock.Stock < cartItem.Quantity)
                {
                    TempData["ErrorMessage"] = $"Sorry, the product '{cartItem.ProductName}' is out of stock or the requested quantity is not available. Please update your cart.";
                    return RedirectToAction("Index", "Cart");
                }
            }

            var userInDb = await db.Users.FindAsync(userId);
            if (userInDb != null)
            {
                if (!string.IsNullOrWhiteSpace(model.CurrentUser.Address))
                {
                    userInDb.Address = model.CurrentUser.Address;
                }
                if (!string.IsNullOrWhiteSpace(model.CurrentUser.PhoneNumber))
                {
                    userInDb.PhoneNumber = model.CurrentUser.PhoneNumber;
                }
            }

            var order = new Order
            {
                UserId = userId,
                OrderDate = DateTime.Now,
                OrderStatus = "Pending",
                TotalPrice = cart.Sum(item => item.Total)
            };

            db.Orders.Add(order);
            await db.SaveChangesAsync();

            foreach (var cartItem in cart)
            {
                var orderDetail = new OrderDetail
                {
                    OrderId = order.Id,
                    ProductId = cartItem.ProductId,
                    Quantity = cartItem.Quantity,
                    Price = cartItem.Price
                };
                db.OrderDetails.Add(orderDetail);

                var productToUpdate = await db.Products.FindAsync(cartItem.ProductId);
                if (productToUpdate != null)
                {
                    productToUpdate.Stock -= cartItem.Quantity;
                }
            }

            await db.SaveChangesAsync();

            HttpContext.Session.Remove(CART_KEY);

            return RedirectToAction("ThankYou", new { orderId = order.Id });
        }

     
        public IActionResult ThankYou(int orderId)
        {
            ViewBag.OrderId = orderId;
            return View();
        }
    }
}