global using BlazorElectronics.Shared;
using BlazorElectronics.Server.Caches.Products;
using BlazorElectronics.Server.DbContext;
using BlazorElectronics.Server.Repositories.Cart;
using BlazorElectronics.Server.Repositories.Categories;
using BlazorElectronics.Server.Repositories.Products;
using BlazorElectronics.Server.Repositories.Sessions;
using BlazorElectronics.Server.Repositories.Specs;
using BlazorElectronics.Server.Repositories.Users;
using BlazorElectronics.Server.Services.Cart;
using BlazorElectronics.Server.Services.Categories;
using BlazorElectronics.Server.Services.Features;
using BlazorElectronics.Server.Services.Products;
using BlazorElectronics.Server.Services.Sessions;
using BlazorElectronics.Server.Services.Specs;
using BlazorElectronics.Server.Services.Users;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSingleton<DapperContext>();

builder.Services.AddSingleton<ICategoryCache, CategoryCache>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<ICategoryService, CategoryService>();

builder.Services.AddSingleton<IFeaturesCache, FeaturesCache>();
builder.Services.AddScoped<IFeaturesService, FeaturesService>();

builder.Services.AddSingleton<IProductCache, ProductCache>();
builder.Services.AddScoped<IProductDetailsRepository, ProductDetailsRepository>();
builder.Services.AddScoped<IProductSearchRepository, ProductSearchRepository>();
builder.Services.AddScoped<IProductService, ProductService>();

//builder.Services.AddSingleton<ISpecCache, SpecCache>();
builder.Services.AddScoped<ISpecLookupRepository, SpecLookupRepository>();
builder.Services.AddScoped<ISpecLookupService, SpecLookupService>();

builder.Services.AddScoped<ICartRepository, CartRepository>();
builder.Services.AddScoped<ICartService, CartService>();

builder.Services.AddScoped<IUserAccountService, UserAccountService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();

builder.Services.AddScoped<ISessionService, SessionService>();
builder.Services.AddScoped<ISessionRepository, SessionRepository>();

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