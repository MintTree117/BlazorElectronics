using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using BlazorElectronics.Client;
using BlazorElectronics.Client.Services.Categories;
using Blazored.LocalStorage;
using BlazorElectronics.Client.Services.Features;
using BlazorElectronics.Client.Services.Users;
using BlazorElectronics.Client.Services.Users.Admin;
using BlazorElectronics.Client.Services.Users.Admin.Category;
using BlazorElectronics.Client.Services.Users.Admin.Features;
using BlazorElectronics.Client.Services.Users.Admin.SpecLookups;
using BlazorElectronics.Client.Services.Users.Admin.Vendors;
using BlazorElectronics.Client.Services.Users.Cart;

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
builder.Services.AddScoped<IAdminCategoryServiceClient, AdminCategoryServiceClient>();
builder.Services.AddScoped<IAdminFeaturesServiceClient, AdminFeaturesServiceClient>();
builder.Services.AddScoped<IAdminSpecsServiceClient, AdminSpecsServiceClient>();
builder.Services.AddScoped<IAdminVendorServiceClient, AdminVendorServiceClient>();

builder.Logging.SetMinimumLevel( LogLevel.Error ); // Set the minimum level of logging

await builder.Build().RunAsync();