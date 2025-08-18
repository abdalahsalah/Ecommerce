namespace Ecommerce_MVC_project.Models
{
    public class DashboardViewModel
    {
        public int TotalUsers { get; set; }
        public int TotalProducts { get; set; }
        public int TotalOrders { get; set; }
        public decimal TotalRevenue { get; set; }
        public int ProductsInStock { get; set; }
        public int PendingOrders { get; set; }

        public List<string> MonthlySalesLabels { get; set; } = new List<string>();
        public List<decimal> MonthlySalesData { get; set; } = new List<decimal>();

        public List<string> ProductsPerCategoryLabels { get; set; } = new List<string>();
        public List<int> ProductsPerCategoryData { get; set; } = new List<int>();
    }
}