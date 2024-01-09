global using BlazorElectronics.Shared;
using System.Configuration;
using System.Data;
using BlazorElectronics.Server.Api.Interfaces;
using BlazorElectronics.Server.Core;
using BlazorElectronics.Server.Core.Interfaces;
using BlazorElectronics.Server.Core.Services;
using BlazorElectronics.Server.Data;
using BlazorElectronics.Server.Data.Repositories;
using Microsoft.Data.SqlClient;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSingleton<DapperContext>();

builder.Services.AddScoped<IEmailService, EmailService>();

builder.Services.AddSingleton<ICategoryService, CategoryService>();
builder.Services.AddSingleton<ICategoryRepository, CategoryRepository>();

builder.Services.AddSingleton<IFeaturesService, FeaturesService>();
builder.Services.AddSingleton<IFeaturesRepository, FeaturesRepository>();

builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IProductSearchRepository, ProductSearchRepository>();
builder.Services.AddScoped<IProductServiceCustomer, ProductServiceCustomer>();
builder.Services.AddScoped<IProductServiceAdmin, ProductServiceAdmin>();
builder.Services.AddScoped<IProductSeedService, ProductSeedService>();

builder.Services.AddScoped<IPromoService, PromoService>();
builder.Services.AddScoped<IPromoRepository, PromoRepository>();

builder.Services.AddScoped<IReviewRepository, ReviewRepository>();
builder.Services.AddScoped<IReviewService, ReviewService>();
builder.Services.AddScoped<IReviewSeedService, ReviewSeedService>();

builder.Services.AddScoped<ICartRepository, CartRepository>();
builder.Services.AddScoped<ICartService, CartService>();

builder.Services.AddScoped<IUserAccountService, UserAccountService>();
builder.Services.AddScoped<IUserSeedService, UserSeedService>();
builder.Services.AddScoped<IUserAccountRepository, UserAccountRepository>();

builder.Services.AddScoped<ISessionService, SessionService>();
builder.Services.AddScoped<ISessionRepository, SessionRepository>();

builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();

builder.Services.AddScoped<IPaymentService, PaymentService>();

builder.Services.AddSingleton<IVendorRepository, VendorRepository>();
builder.Services.AddSingleton<IVendorService, VendorService>();

builder.Services.AddSingleton<ISpecRepository, SpecRepository>();
builder.Services.AddSingleton<ISpecLookupsService, SpecLookupsService>();

WebApplication app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseBlazorFrameworkFiles();
app.UseStaticFiles();

app.UseRouting();

app.MapRazorPages();
app.MapControllers();
app.MapFallbackToFile("index.html");

app.Run();