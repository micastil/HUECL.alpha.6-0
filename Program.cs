using HUECL.alpha._6_0.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using HUECL.alpha._6_0.Areas.Identity.Data;
using HUECL.alpha._6_0.Models.Repositories;
using System.Globalization;
using Microsoft.AspNetCore.Localization;
using Microsoft.Data.SqlClient;
using Microsoft.AspNetCore.Authentication;
using HUECL.alpha._6_0.Areas.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Mvc.Razor;
using HUECL.alpha._6_0.Areas.Identity.Pages;
using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
using HUECL.alpha._6_0.Interfaces;
using HUECL.alpha._6_0.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();


var conStrBuilder = new SqlConnectionStringBuilder(
        builder.Configuration.GetConnectionString("External"));
conStrBuilder.Password = builder.Configuration["ExternalPassword"];
var connection = conStrBuilder.ConnectionString;

//builder.Services.AddDbContext<AppDbContext>(options => {
//    options.UseSqlServer(builder.Configuration["ConnectionStrings:External"]);
//});

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(connection);
});

builder.Services.AddDefaultIdentity<ApplicationUser>(options =>
    {
        options.SignIn.RequireConfirmedAccount = false;
        options.User.RequireUniqueEmail = true;
        options.Password.RequireDigit = true;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequireUppercase = true;
    })
    .AddRoles<IdentityRole>()
    .AddErrorDescriber<SpanishIdentityErrorDescriber>()
    .AddEntityFrameworkStores<AppDbContext>();

builder.Services.AddControllers(
    options => options.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true);

builder.Services.AddScoped<IClaimsTransformation, ClaimsTransformer>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<ISaleRepository, SaleRepository>();
builder.Services.AddScoped<ISaleDeliveryRepository, SaleDeliveryRepository>();
builder.Services.AddScoped<ISaleInvoiceRepository, SaleInvoiceRepository>();

builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromSeconds(1800);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddAuthorization(o =>
{
    o.AddPolicy("IsSuperUser", o => o.RequireRole(GlobalRoleName.Super));
    o.AddPolicy("IsManager", o => o.RequireRole(GlobalRoleName.Manager));
    o.AddPolicy("IsSales", o => o.RequireRole(GlobalRoleName.Sales));
    o.AddPolicy("IsGuest", o => o.RequireRole(GlobalRoleName.Guest));

    o.AddPolicy("CanRead", o => o.RequireClaim(GlobalPermissionType.CanRead, "True"));
    o.AddPolicy("CanWrite", o => o.RequireClaim(GlobalPermissionType.CanWrite, "True"));
    o.AddPolicy("CanDelete", o => o.RequireClaim(GlobalPermissionType.CanDelete, "True"));
});

builder.Services.Configure<RazorViewEngineOptions>(options =>
{
    //Add view search paths for Maintenance folder
    //options.ViewLocationFormats.Add("/Views/Maintenance/{1}/{0}.cshtml");
    //options.ViewLocationFormats.Add("/Views/Maintenance/Shared/{0}.cshtml");
});

builder.Services.AddDataProtection();
builder.Services.AddScoped<ICustomDataProtector, CustomDataProtector>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}

var cultureInfo = new CultureInfo("es-CL");
cultureInfo.NumberFormat.CurrencySymbol = "$";
cultureInfo.NumberFormat.NumberDecimalSeparator = ",";

CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;

app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication(); ;
app.UseAuthorization();

app.UseSession();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

//app.UseEndpoints(
//    endpoints => 
//    {
//        endpoints.MapControllerRoute(
//            name: "Maintenance",
//            pattern: "Maintenance/{controller=Home}/{action=Index}/{id?}");
//        endpoints.MapRazorPages();
//    }
//);

app.MapRazorPages();

//app.UseEndpoints(
//    endpoints =>
//    {
//        endpoints.MapRazorPages();
//    }
//);

DBSeed.SeedBase(app);
DBSeed.SeedCategories(app);
await DBSeed.SeedSuperUser(app, app.Configuration);

app.Run();
