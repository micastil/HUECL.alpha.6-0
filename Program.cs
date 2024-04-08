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
        options.Password.RequireNonAlphanumeric = true;
        options.Password.RequireUppercase = true;
    })
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

app.UseEndpoints(
    endpoints => endpoints.MapRazorPages()
);

DBSeed.SeedBase(app);
DBSeed.SeedCategories(app);

app.Run();
