// --- الـ 'usings' اللازمة ---
using Ecommerce_MVC_project.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Ecommerce_MVC_project.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private readonly EFContext db;

        public AccountController(EFContext context)
        {
            db = context;
        }



        private int GetCurrentUserId()
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.Parse(userIdString);
        }


        public async Task<IActionResult> Index()
        {
            var userId = GetCurrentUserId();
            var user = await db.Users.FindAsync(userId);
            if (user == null)
            {
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                return RedirectToAction("Index", "Home");
            }
            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(User model)
        {
            var userId = GetCurrentUserId();
            if (userId != model.Id)
            {
                return BadRequest("Invalid user data.");
            }

            var userInDb = await db.Users.FindAsync(userId);
            if (userInDb == null)
            {
                return NotFound("User not found.");
            }

            if (userInDb.Email != model.Email)
            {
                if (await db.Users.AnyAsync(u => u.Email == model.Email && u.Id != userId))
                {
                    ModelState.AddModelError("Email", "This email is already in use by another account.");
                    return View(model);
                }
            }

            userInDb.Name = model.Name;
            userInDb.Email = model.Email;
            userInDb.Age = model.Age;
            userInDb.PhoneNumber = model.PhoneNumber;
            userInDb.Address = model.Address;

            db.Users.Update(userInDb);
            await db.SaveChangesAsync();


            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userInDb.Id.ToString()),
                new Claim(ClaimTypes.Name, userInDb.Name), 
                new Claim(ClaimTypes.Email, userInDb.Email), 
                new Claim(ClaimTypes.Role, userInDb.Role)
            };
            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));


            TempData["SuccessMessage"] = "Your profile has been updated successfully.";
            return RedirectToAction("Index");
        }


        public async Task<IActionResult> MyOrders()
        {
            var userId = GetCurrentUserId();
            var orders = await db.Orders
                .Where(o => o.UserId == userId)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();

            return View(orders);
        }

        public async Task<IActionResult> OrderDetails(int id)
        {
            var userId = GetCurrentUserId();
            var order = await db.Orders
                .Include(o => o.OrderDetails)
                .ThenInclude(od => od.Product)
                .FirstOrDefaultAsync(o => o.Id == id && o.UserId == userId);

            if (order == null)
            {
                return Forbid(); 
            }
            return View(order);
        }
    }
}