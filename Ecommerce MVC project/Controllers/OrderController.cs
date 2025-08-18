using Ecommerce_MVC_project.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace Ecommerce_MVC_project.Controllers
{
    [Authorize(Roles = "Admin")] 
    public class OrderController : Controller
    {
        private readonly EFContext db;

        public OrderController(EFContext context)
        {
            db = context;
        }

        public async Task<IActionResult> Index()
        {
            var orders = await db.Orders
                .Include(o => o.User) 
                .OrderByDescending(o => o.OrderDate) 
                .ToListAsync();
            return View(orders);
        }

        public async Task<IActionResult> Details(int id)
        {
            var order = await db.Orders
                .Include(o => o.User) 
                .Include(o => o.OrderDetails) 
                .ThenInclude(od => od.Product) 
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(int orderId, string status)
        {
            var order = await db.Orders.FindAsync(orderId);
            if (order == null)
            {
                return NotFound();
            }

        
            if (!string.IsNullOrEmpty(status))
            {
                order.OrderStatus = status;
                await db.SaveChangesAsync();
                TempData["SuccessMessage"] = $"Order #{orderId} status has been updated to '{status}'.";
            }
            else
            {
                TempData["ErrorMessage"] = "Invalid status provided.";
            }

            return RedirectToAction("Details", new { id = orderId });
        }
    }
}