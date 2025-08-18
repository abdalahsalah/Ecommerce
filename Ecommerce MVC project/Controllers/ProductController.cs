using Ecommerce_MVC_project.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Ecommerce_MVC_project.Controllers
{
    public class ProductController : Controller
    {
        private readonly EFContext db;
        private readonly IWebHostEnvironment _hostEnvironment;

        public ProductController(EFContext context, IWebHostEnvironment hostEnvironment)
        {
            db = context;
            _hostEnvironment = hostEnvironment;
        }

        public async Task<IActionResult> Index()
        {
            var products = await db.Products
                .Include(p => p.Category)
                .Include(p => p.Brand)
                .ToListAsync();
            return View(products);
        }

        private async Task PrepareDropdowns()
        {
            ViewBag.Categories = await db.Categories.OrderBy(c => c.Name).ToListAsync();
            ViewBag.Brands = await db.Brands.OrderBy(b => b.Name).ToListAsync();
            ViewBag.Users = await db.Users.Where(u => u.Role.ToLower() == "admin").ToListAsync();
        }

        public async Task<IActionResult> Create()
        {
            await PrepareDropdowns();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Product product, IFormFile? imageFile)
        {
            if (ModelState.IsValid)
            {
                if (imageFile != null)
                {
                    string wwwRootPath = _hostEnvironment.WebRootPath;
                    string imageFolderPath = Path.Combine(wwwRootPath, "images", "products");
                    if (!Directory.Exists(imageFolderPath)) { Directory.CreateDirectory(imageFolderPath); }
                    string uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
                    string imageFinalPath = Path.Combine(imageFolderPath, uniqueFileName);
                    using (var fileStream = new FileStream(imageFinalPath, FileMode.Create)) { await imageFile.CopyToAsync(fileStream); }
                    product.ImagePath = "/images/products/" + uniqueFileName;
                }
                else { product.ImagePath = "/images/default-product.png"; }

                product.CreatedAt = DateTime.Now;
                db.Products.Add(product);
                await db.SaveChangesAsync();
                TempData["SuccessMessage"] = "Product has been created successfully!";
                return RedirectToAction(nameof(Index));
            }
            await PrepareDropdowns();
            return View(product);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var product = await db.Products.FindAsync(id);
            if (product == null) { return NotFound(); }
            await PrepareDropdowns();
            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Product product, IFormFile? imageFile)
        {
            if (id != product.Id) { return BadRequest(); }
            if (ModelState.IsValid)
            {
                if (imageFile != null)
                {
                    var existingProduct = await db.Products.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);
                    if (existingProduct != null && !string.IsNullOrEmpty(existingProduct.ImagePath) && existingProduct.ImagePath != "/images/default-product.png")
                    {
                        string oldImagePath = Path.Combine(_hostEnvironment.WebRootPath, existingProduct.ImagePath.TrimStart('/'));
                        if (System.IO.File.Exists(oldImagePath)) { System.IO.File.Delete(oldImagePath); }
                    }
                    string imageFolderPath_new = Path.Combine(_hostEnvironment.WebRootPath, "images", "products");
                    string uniqueFileName_new = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
                    string imageFinalPath_new = Path.Combine(imageFolderPath_new, uniqueFileName_new);
                    using (var fs = new FileStream(imageFinalPath_new, FileMode.Create)) { await imageFile.CopyToAsync(fs); }
                    product.ImagePath = "/images/products/" + uniqueFileName_new;
                }

                db.Update(product);
                await db.SaveChangesAsync();
                TempData["SuccessMessage"] = "Product has been updated successfully!";
                return RedirectToAction(nameof(Index));
            }
            await PrepareDropdowns();
            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var product = await db.Products.FindAsync(id);
            if (product == null) { return NotFound(); }

            if (!string.IsNullOrEmpty(product.ImagePath) && product.ImagePath != "/images/default-product.png")
            {
                string oldImagePath = Path.Combine(_hostEnvironment.WebRootPath, product.ImagePath.TrimStart('/'));
                if (System.IO.File.Exists(oldImagePath)) { System.IO.File.Delete(oldImagePath); }
            }

            db.Products.Remove(product);
            await db.SaveChangesAsync();
            TempData["SuccessMessage"] = "Product has been deleted successfully!";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Details(int id)
        {
            var product = await db.Products
                .Include(p => p.Category)
                .Include(p => p.Brand)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null) { return NotFound(); }
            return View(product);
        }
    }
}