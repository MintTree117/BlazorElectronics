using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using BlazorElectronics.Client;
using BlazorElectronics.Client.Services.Categories;
using BlazorElectronics.Client.Services.Products;

var builder = WebAssemblyHostBuilder.CreateDefault( args );
builder.RootComponents.Add<App>( "#app" );
builder.RootComponents.Add<HeadOutlet>( "head::after" );

builder.Services.AddScoped( sp => new HttpClient { BaseAddress = new Uri( builder.HostEnvironment.BaseAddress ) } );
builder.Services.AddScoped<ICategoryServiceClient, CategoryServiceClient>();
builder.Services.AddScoped<IProductServiceClient, ProductServiceClient>();

await builder.Build().RunAsync();