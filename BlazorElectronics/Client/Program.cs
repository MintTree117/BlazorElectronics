using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using BlazorElectronics.Client;
using BlazorElectronics.Client.Services.Categories;
using Blazored.LocalStorage;
using BlazorElectronics.Client.Services.Admin;
using BlazorElectronics.Client.Services.Features;
using BlazorElectronics.Client.Services.Users;
using BlazorElectronics.Client.Services.Users.Cart;
using BlazorElectronics.Shared;
using BlazorElectronics.Shared.Categories;
using BlazorElectronics.Shared.Features;
using BlazorElectronics.Shared.SpecLookups;
using BlazorElectronics.Shared.Vendors;

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
builder.Services.AddScoped<IAdminCrudService<CategoryView, CategoryEdit>, AdminCrudService<CategoryView, CategoryEdit>>();
builder.Services.AddScoped<IAdminCrudService<CrudView, FeaturedDealEdit>, AdminCrudService<CrudView, FeaturedDealEdit>>();
builder.Services.AddScoped<IAdminCrudService<CrudView, FeatureEdit>, AdminCrudService<CrudView, FeatureEdit>>();
builder.Services.AddScoped<IAdminCrudService<CrudView, SpecEdit>, AdminCrudService<CrudView, SpecEdit>>();
builder.Services.AddScoped<IAdminCrudService<CrudView, VendorEdit>, AdminCrudService<CrudView, VendorEdit>>();
builder.Services.AddScoped<IAdminSeedService, AdminSeedService>();
builder.Services.AddScoped<IAdminCategoryHelper, AdminCategoryHelper>();
builder.Services.AddScoped<IAdminBulkServiceClient, AdminBulkServiceClient>();


builder.Logging.SetMinimumLevel( LogLevel.Error ); // Set the minimum level of logging

await builder.Build().RunAsync();