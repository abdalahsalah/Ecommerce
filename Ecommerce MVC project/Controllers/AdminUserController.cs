using Ecommerce_MVC_project.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Ecommerce_MVC_project.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminUserController : Controller
    {
        private readonly EFContext db;

        public AdminUserController(EFContext context)
        {
            db = context;
        }

        public async Task<IActionResult> Index()
        {
            var users = await db.Users.ToListAsync();
            return View(users);
        }

        public async Task<IActionResult> Details(int id)
        {
            var user = await db.Users
                .Include(u => u.Orders) 
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateRole(int userId, string newRole)
        {
            var user = await db.Users.FindAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            if (newRole == "Admin" || newRole == "Customer")
            {
                user.Role = newRole;
                await db.SaveChangesAsync();
                TempData["SuccessMessage"] = $"User '{user.Name}' role has been updated to '{newRole}'.";
            }
            else
            {
                TempData["ErrorMessage"] = "Invalid role specified.";
            }

            return RedirectToAction("Details", new { id = userId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var user = await db.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

       
            db.Users.Remove(user);
            await db.SaveChangesAsync();

            TempData["SuccessMessage"] = $"User '{user.Name}' has been deleted.";
            return RedirectToAction("Index");
        }
    }
}