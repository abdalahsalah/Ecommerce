using Ecommerce_MVC_project.Models; // << تأكد من استبدال هذا باسم الـ namespace لمشروعك إذا كان مختلفاً
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims; // << لإضافة Roles لاحقاً
using Microsoft.Extensions.DependencyInjection; // << قد تحتاجه


var builder = WebApplication.CreateBuilder(args);

// --- 1. تسجيل الخدمات في حاوية الـ DI Container ---

builder.Services.AddControllersWithViews();

// تسجيل DbContext كخدمة
builder.Services.AddDbContext<EFContext>();

// إعدادات الـ Session لسلة التسوق
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// خدمة ضرورية للوصول لـ HttpContext من أماكن أخرى مثل الـ ViewComponents
builder.Services.AddHttpContextAccessor();

// إعدادات الـ Authentication باستخدام الكوكيز
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/User/Login";          // المسار لصفحة تسجيل الدخول
        options.AccessDeniedPath = "/Home/AccessDenied"; // المسار لصفحة رفض الوصول (يجب إنشاؤها)
        options.ExpireTimeSpan = TimeSpan.FromDays(7);  // صلاحية الكوكي 7 أيام
    });

// إعدادات الـ Authorization Policies (متقدم، يمكن إضافته لاحقاً لو احتجت)
// مثال:
// builder.Services.AddAuthorization(options =>
// {
//     options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
// });


var app = builder.Build();

// --- 2. إعداد الـ HTTP Request Pipeline ---

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

// الترتيب هنا مهم جداً
app.UseRouting();

app.UseSession(); // مهم لسلة التسوق

app.UseAuthentication(); // <-- خطوة 1: تحديد هوية المستخدم (من هو؟)
app.UseAuthorization();  // <-- خطوة 2: التحقق من صلاحيات المستخدم (ماذا يمكنه أن يفعل؟)

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();