using BlazorElectronics.Server.DbContext;
using BlazorElectronics.Server.Repositories.Cart;
using BlazorElectronics.Server.Repositories.Categories;
using BlazorElectronics.Server.Repositories.Features;
using BlazorElectronics.Server.Repositories.Products;
using BlazorElectronics.Server.Repositories.Specs;
using BlazorElectronics.Server.Repositories.Users;
using BlazorElectronics.Server.Services.Cart;
using BlazorElectronics.Server.Services.Categories;
using BlazorElectronics.Server.Services.Features;
using BlazorElectronics.Server.Services.Products;
using BlazorElectronics.Server.Services.Specs;
using BlazorElectronics.Server.Services.Users;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

WebApplicationBuilder builder = WebApplication.CreateBuilder( args );

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

builder.Services.AddSingleton<DapperContext>();

builder.Services.AddSingleton<ICategoryCache, CategoryCache>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<ICategoryService, CategoryService>();

builder.Services.AddSingleton<IFeaturesCache, FeaturesCache>();
builder.Services.AddScoped<IFeaturesService, FeaturesService>();
builder.Services.AddScoped<IFeaturedProductsRepository, FeaturedProductsRepository>();
builder.Services.AddScoped<IFeaturedDealsRepository, FeaturedDealsRepository>();

builder.Services.AddSingleton<IProductCache, ProductCache>();
builder.Services.AddScoped<IProductDetailsRepository, ProductDetailsRepository>();
builder.Services.AddScoped<IProductSearchRepository, ProductSearchRepository>();
builder.Services.AddScoped<IProductService, ProductService>();

builder.Services.AddSingleton<ISpecCache, SpecCache>();
builder.Services.AddScoped<ISpecLookupRepository, SpecLookupRepository>();
builder.Services.AddScoped<ISpecDescrRepository, SpecDescrRepository>();
builder.Services.AddScoped<ISpecService, SpecService>();

builder.Services.AddScoped<ICartRepository, CartRepository>();
builder.Services.AddScoped<ICartService, CartService>();

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();

builder.Services.AddAuthentication( JwtBearerDefaults.AuthenticationScheme )
    .AddJwtBearer( options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey( System.Text.Encoding.UTF8.GetBytes(
                builder.Configuration.GetSection( "AppSettings:Token" ).Value! ) ),
            ValidateIssuer = false,
            ValidateAudience = false
        };
    } );

WebApplication app = builder.Build();

// Configure the HTTP request pipeline.
if ( app.Environment.IsDevelopment() ) {
    app.UseWebAssemblyDebugging();
}
else {
    app.UseExceptionHandler( "/Error" );
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseBlazorFrameworkFiles();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();
app.MapControllers();
app.MapFallbackToFile( "index.html" );

app.Run();