using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using BlazorElectronics.Client;
using BlazorElectronics.Client.Services.Categories;
using Blazored.LocalStorage;
using BlazorElectronics.Client.Services.Features;
using BlazorElectronics.Client.Services.Users;
using BlazorElectronics.Client.Services.Users.Admin;
using BlazorElectronics.Client.Services.Users.Cart;
using BlazorElectronics.Shared.Categories;

var builder = WebAssemblyHostBuilder.CreateDefault( args );
builder.RootComponents.Add<App>( "#app" );
builder.RootComponents.Add<HeadOutlet>( "head::after" );

builder.Services.AddBlazoredLocalStorage();
builder.Services.AddScoped( sp => new HttpClient { BaseAddress = new Uri( builder.HostEnvironment.BaseAddress ) } );

builder.Services.AddScoped<IFeaturesServiceClient, FeaturesServiceClient>();
builder.Services.AddScoped<ICategoryServiceClient, CategoryServiceClient>();
builder.Services.AddScoped<ICartServiceClient, CartServiceClient>();

builder.Services.AddScoped<IUserServiceClient, UserServiceClient>();

builder.Services.AddScoped<IAdminServiceClient, AdminServiceClient>();
builder.Services.AddScoped<IAdminCrudService<CategoryViewDto,CategoryEditDto>, AdminCrudService<CategoryViewDto, CategoryEditDto>>();


builder.Logging.SetMinimumLevel( LogLevel.Error ); // Set the minimum level of logging

await builder.Build().RunAsync();