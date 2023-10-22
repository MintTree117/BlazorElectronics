using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using BlazorElectronics.Client;
using BlazorElectronics.Client.Services.Categories;
using BlazorElectronics.Client.Services.Products;
using Blazored.LocalStorage;
using BlazorElectronics.Client.Services.Cart;
using BlazorElectronics.Client.Services.Features;
using BlazorElectronics.Client.Services.Users;

var builder = WebAssemblyHostBuilder.CreateDefault( args );
builder.RootComponents.Add<App>( "#app" );
builder.RootComponents.Add<HeadOutlet>( "head::after" );

builder.Services.AddBlazoredLocalStorage();
builder.Services.AddScoped( sp => new HttpClient { BaseAddress = new Uri( builder.HostEnvironment.BaseAddress ) } );
builder.Services.AddScoped<IFeaturesServiceClient, FeaturesServiceClient>();
builder.Services.AddScoped<ICategoryServiceClient, CategoryServiceClient>();
builder.Services.AddScoped<IProductServiceClient, ProductServiceClient>();
builder.Services.AddScoped<ICartService, CartService>();
builder.Services.AddScoped<IUserServiceClient, UserServiceClient>();

await builder.Build().RunAsync();