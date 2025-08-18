using Ecommerce_MVC_project.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Authorization;

namespace Ecommerce_MVC_project.Controllers
{
    [Authorize(Roles = "Admin")]
    public class BrandController : Controller
    {
        private readonly EFContext db;
        private readonly IWebHostEnvironment _hostEnvironment;

        public BrandController(EFContext context, IWebHostEnvironment hostEnvironment)
        {
            db = context;
            _hostEnvironment = hostEnvironment;
        }

        public async Task<IActionResult> Index()
        {
            var model = await db.Brands.Include(b => b.User).ToListAsync();
            return View(model);
        }

        public async Task<IActionResult> Create()
        {
            ViewBag.Users = await db.Users.Where(u => u.Role.ToLower() == "admin").ToListAsync();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Brand brand, IFormFile? imageFile)
        {
            if (ModelState.IsValid)
            {
                if (imageFile != null && imageFile.Length > 0)
                {
                    string wwwRootPath = _hostEnvironment.WebRootPath;
                    string imageFolderPath = Path.Combine(wwwRootPath, "images", "brands");
                    if (!Directory.Exists(imageFolderPath)) { Directory.CreateDirectory(imageFolderPath); }
                    string uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
                    string imageFinalPath = Path.Combine(imageFolderPath, uniqueFileName);
                    using (var fileStream = new FileStream(imageFinalPath, FileMode.Create)) { await imageFile.CopyToAsync(fileStream); }
                    brand.ImagePath = "/images/brands/" + uniqueFileName;
                }
                else
                {
                    brand.ImagePath = "/images/default-brand.png";
                }
                db.Brands.Add(brand);
                await db.SaveChangesAsync();
                TempData["SuccessMessage"] = "Brand has been created successfully!";
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Users = await db.Users.Where(u => u.Role.ToLower() == "admin").ToListAsync();
            return View(brand);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var brand = await db.Brands.FindAsync(id);
            if (brand == null) { return NotFound(); }
            ViewBag.Users = await db.Users.Where(u => u.Role.ToLower() == "admin").ToListAsync();
            return View(brand);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Brand brand, IFormFile? imageFile)
        {
            if (id != brand.Id) { return BadRequest(); }

            if (ModelState.IsValid)
            {
                if (imageFile != null && imageFile.Length > 0)
                {
                    var existingBrand = await db.Brands.AsNoTracking().FirstOrDefaultAsync(b => b.Id == id);
                    if (existingBrand != null && !string.IsNullOrEmpty(existingBrand.ImagePath) && existingBrand.ImagePath != "/images/default-brand.png")
                    {
                        string wwwRootPath = _hostEnvironment.WebRootPath;
                        string oldImagePath = Path.Combine(wwwRootPath, existingBrand.ImagePath.TrimStart('/'));
                        if (System.IO.File.Exists(oldImagePath)) { System.IO.File.Delete(oldImagePath); }
                    }

                    string wwwRootPath_new = _hostEnvironment.WebRootPath;
                    string imageFolderPath_new = Path.Combine(wwwRootPath_new, "images", "brands");
                    string uniqueFileName_new = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
                    string imageFinalPath_new = Path.Combine(imageFolderPath_new, uniqueFileName_new);
                    using (var fileStream = new FileStream(imageFinalPath_new, FileMode.Create)) { await imageFile.CopyToAsync(fileStream); }
                    brand.ImagePath = "/images/brands/" + uniqueFileName_new;
                }

                try
                {
                    db.Update(brand);
                    await db.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Brand has been updated successfully!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!db.Brands.Any(e => e.Id == brand.Id)) { return NotFound(); } else { throw; }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Users = await db.Users.Where(u => u.Role.ToLower() == "admin").ToListAsync();
            return View(brand);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var brand = await db.Brands.FindAsync(id);
            if (brand == null) { return NotFound(); }

            if (!string.IsNullOrEmpty(brand.ImagePath) && brand.ImagePath != "/images/default-brand.png")
            {
                string wwwRootPath = _hostEnvironment.WebRootPath;
                string oldImagePath = Path.Combine(wwwRootPath, brand.ImagePath.TrimStart('/'));
                if (System.IO.File.Exists(oldImagePath)) { System.IO.File.Delete(oldImagePath); }
            }

            db.Brands.Remove(brand);
            await db.SaveChangesAsync();
            TempData["SuccessMessage"] = "Brand has been deleted successfully!";
            return RedirectToAction(nameof(Index));
        }
    }
}