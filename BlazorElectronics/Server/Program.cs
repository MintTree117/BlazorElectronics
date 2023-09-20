using BlazorElectronics.Server.Caches.Products;
using BlazorElectronics.Server.DbContext;
using BlazorElectronics.Server.Repositories;
using BlazorElectronics.Server.Services;
using BlazorElectronics.Server.Services.Products;

WebApplicationBuilder builder = WebApplication.CreateBuilder( args );

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();
builder.Services.AddSingleton<DapperContext>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddSingleton<IProductCache, ProductCache>();
builder.Services.AddScoped<IProductService, ProductService>();

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


app.MapRazorPages();
app.MapControllers();
app.MapFallbackToFile( "index.html" );

app.Run();