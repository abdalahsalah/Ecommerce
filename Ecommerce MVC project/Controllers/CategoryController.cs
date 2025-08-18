using Ecommerce_MVC_project.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.Linq;
using Microsoft.AspNetCore.Authorization;

namespace Ecommerce_MVC_project.Controllers
{
    [Authorize(Roles = "Admin")]
    public class CategoryController : Controller
    {
        private readonly EFContext db;
        private readonly IWebHostEnvironment _hostEnvironment;

        public CategoryController(EFContext context, IWebHostEnvironment hostEnvironment)
        {
            db = context;
            _hostEnvironment = hostEnvironment;
        }

        public IActionResult Index()
        {
            var model = db.Categories.Include(c => c.User).ToList();
            return View(model);
        }

        public IActionResult Create()
        {
            var model = db.Users.Where(u => u.Role.ToLower() == "admin").ToList();
            ViewBag.Users = model;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Category category, IFormFile? imageFile)
        {
            if (imageFile != null && imageFile.Length > 0)
            {
                string wwwRootPath = _hostEnvironment.WebRootPath;
                string imageFolderPath = Path.Combine(wwwRootPath, "images", "categories");
                if (!Directory.Exists(imageFolderPath))
                {
                    Directory.CreateDirectory(imageFolderPath);
                }
                string uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
                string imageFinalPath = Path.Combine(imageFolderPath, uniqueFileName);
                using (var fileStream = new FileStream(imageFinalPath, FileMode.Create))
                {
                    await imageFile.CopyToAsync(fileStream);
                }
                category.ImagePath = "/images/categories/" + uniqueFileName;
            }
            else
            {
                category.ImagePath = "/images/default-category.png";
            }
            db.Categories.Add(category);
            await db.SaveChangesAsync();
            TempData["SuccessMessage"] = "Category has been created successfully!";
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Edit(int id)
        {
            var category = await db.Categories.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }
            ViewBag.Users = await db.Users.Where(u => u.Role.ToLower() == "admin").ToListAsync();
            return View(category);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Category category, IFormFile? imageFile)
        {
            if (id != category.Id)
            {
                return BadRequest(); 
            }

            if (ModelState.IsValid)
            {
                if (imageFile != null && imageFile.Length > 0)
                {
                    var existingCategory = await db.Categories.AsNoTracking().FirstOrDefaultAsync(c => c.Id == id);
                    if (existingCategory != null && !string.IsNullOrEmpty(existingCategory.ImagePath) && existingCategory.ImagePath != "/images/default-category.png")
                    {
                        string wwwRootPath = _hostEnvironment.WebRootPath;
                        string oldImagePath = Path.Combine(wwwRootPath, existingCategory.ImagePath.TrimStart('/'));
                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }

                    string wwwRootPath_new = _hostEnvironment.WebRootPath;
                    string imageFolderPath_new = Path.Combine(wwwRootPath_new, "images", "categories");
                    string uniqueFileName_new = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
                    string imageFinalPath_new = Path.Combine(imageFolderPath_new, uniqueFileName_new);
                    using (var fileStream = new FileStream(imageFinalPath_new, FileMode.Create))
                    {
                        await imageFile.CopyToAsync(fileStream);
                    }
                    category.ImagePath = "/images/categories/" + uniqueFileName_new;
                }

                try
                {
                    db.Update(category);
                    await db.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Category has been updated successfully!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!db.Categories.Any(e => e.Id == category.Id)) { return NotFound(); }
                    else { throw; }
                }
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Users = await db.Users.Where(u => u.Role.ToLower() == "admin").ToListAsync();
            return View(category);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var category = await db.Categories.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }

            if (!string.IsNullOrEmpty(category.ImagePath) && category.ImagePath != "/images/default-category.png")
            {
                string wwwRootPath = _hostEnvironment.WebRootPath;
                string oldImagePath = Path.Combine(wwwRootPath, category.ImagePath.TrimStart('/'));
                if (System.IO.File.Exists(oldImagePath))
                {
                    System.IO.File.Delete(oldImagePath);
                }
            }
            db.Categories.Remove(category);
            await db.SaveChangesAsync();
            TempData["SuccessMessage"] = "Category has been deleted successfully!";
            return RedirectToAction(nameof(Index));
        }
    }
}