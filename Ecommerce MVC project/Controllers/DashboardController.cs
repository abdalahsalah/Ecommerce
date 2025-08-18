using Ecommerce_MVC_project.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace Ecommerce_MVC_project.Controllers
{
    [Authorize(Roles = "Admin")]
    public class DashboardController : Controller
    {
        private readonly EFContext db;

        public DashboardController(EFContext context)
        {
            db = context;
        }

        public async Task<IActionResult> Index()
        {
            var viewModel = new DashboardViewModel();

            viewModel.TotalUsers = await db.Users.CountAsync();
            viewModel.TotalProducts = await db.Products.CountAsync();
            viewModel.TotalOrders = await db.Orders.CountAsync();
            viewModel.TotalRevenue = await db.Orders.Where(o => o.OrderStatus == "Delivered").SumAsync(o => o.TotalPrice);
            viewModel.ProductsInStock = await db.Products.SumAsync(p => p.Stock);
            viewModel.PendingOrders = await db.Orders.CountAsync(o => o.OrderStatus == "Pending" || o.OrderStatus == "Processing");

            for (int i = 5; i >= 0; i--)
            {
                var date = DateTime.Now.AddMonths(-i);
                var monthName = date.ToString("MMM yyyy", CultureInfo.InvariantCulture); // "Jan 2024"
                var salesInMonth = await db.Orders
                    .Where(o => o.OrderDate.Year == date.Year && o.OrderDate.Month == date.Month && o.OrderStatus == "Delivered")
                    .SumAsync(o => o.TotalPrice);

                viewModel.MonthlySalesLabels.Add(monthName);
                viewModel.MonthlySalesData.Add(salesInMonth);
            }

            var productsPerCategory = await db.Categories
                .Include(c => c.Products)
                .Select(c => new
                {
                    CategoryName = c.Name,
                    ProductCount = c.Products.Count()
                })
                .Where(c => c.ProductCount > 0)
                .OrderByDescending(c => c.ProductCount)
                .ToListAsync();

            foreach (var item in productsPerCategory)
            {
                viewModel.ProductsPerCategoryLabels.Add(item.CategoryName);
                viewModel.ProductsPerCategoryData.Add(item.ProductCount);
            }

            return View(viewModel);
        }
    }
}