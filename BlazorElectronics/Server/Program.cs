global using BlazorElectronics.Shared;
using BlazorElectronics.Server.DbContext;
using BlazorElectronics.Server.Repositories.Cart;
using BlazorElectronics.Server.Repositories.Categories;
using BlazorElectronics.Server.Repositories.Features;
using BlazorElectronics.Server.Repositories.Products;
using BlazorElectronics.Server.Repositories.Sessions;
using BlazorElectronics.Server.Repositories.SpecLookups;
using BlazorElectronics.Server.Repositories.Users;
using BlazorElectronics.Server.Repositories.Vendors;
using BlazorElectronics.Server.Services.Cart;
using BlazorElectronics.Server.Services.Categories;
using BlazorElectronics.Server.Services.Features;
using BlazorElectronics.Server.Services.Products;
using BlazorElectronics.Server.Services.Sessions;
using BlazorElectronics.Server.Services.Users;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSingleton<DapperContext>();

builder.Services.AddScoped<ICategoryService, CategoryService>();

builder.Services.AddSingleton<IFeaturesCache, FeaturesCache>();
builder.Services.AddScoped<IFeaturesService, FeaturesService>();

builder.Services.AddScoped<IProductDetailsRepository, ProductDetailsRepository>();
builder.Services.AddScoped<IProductSearchRepository, ProductSearchRepository>();
builder.Services.AddScoped<IProductService, ProductService>();

builder.Services.AddScoped<ICartRepository, CartRepository>();
builder.Services.AddScoped<ICartService, CartService>();

builder.Services.AddScoped<IUserAccountService, UserAccountService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();

builder.Services.AddScoped<ISessionService, SessionService>();
builder.Services.AddScoped<ISessionRepository, SessionRepository>();

builder.Services.AddScoped<IVendorRepository, VendorRepository>();

builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IFeaturesRepository, FeaturesRepository>();
builder.Services.AddScoped<ISpecLookupRepository, SpecLookupRepository>();
builder.Services.AddScoped<IVendorRepository, VendorRepository>();

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