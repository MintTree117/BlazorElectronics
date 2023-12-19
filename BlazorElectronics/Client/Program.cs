using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using BlazorElectronics.Client;
using BlazorElectronics.Client.Services.Categories;
using Blazored.LocalStorage;
using BlazorElectronics.Client.Services.Admin;
using BlazorElectronics.Client.Services.Cart;
using BlazorElectronics.Client.Services.Features;
using BlazorElectronics.Client.Services.Products;
using BlazorElectronics.Client.Services.Reviews;
using BlazorElectronics.Client.Services.Specs;
using BlazorElectronics.Client.Services.Users;
using BlazorElectronics.Client.Services.Vendors;
using BlazorElectronics.Shared;
using BlazorElectronics.Shared.Categories;
using BlazorElectronics.Shared.Features;
using BlazorElectronics.Shared.Specs;
using BlazorElectronics.Shared.Vendors;

var builder = WebAssemblyHostBuilder.CreateDefault( args );
builder.RootComponents.Add<App>( "#app" );
builder.RootComponents.Add<HeadOutlet>( "head::after" );

builder.Services.AddBlazoredLocalStorage();
builder.Services.AddScoped( sp => new HttpClient { BaseAddress = new Uri( builder.HostEnvironment.BaseAddress ) } );

builder.Services.AddScoped<ICartServiceClient, CartServiceClient>();
builder.Services.AddScoped<ICategoryServiceClient, CategoryServiceClient>();
builder.Services.AddScoped<IFeaturesServiceClient, FeaturesServiceClient>();
builder.Services.AddScoped<IProductServiceClient, ProductServiceClient>();
builder.Services.AddScoped<IReviewServiceClient, ReviewServiceClient>();
builder.Services.AddScoped<ISpecServiceClient, SpecServiceClient>();
builder.Services.AddScoped<IVendorServiceClient, VendorServiceClient>();

builder.Services.AddScoped<IUserServiceClient, UserServiceClient>();

builder.Services.AddScoped<IAdminServiceClient, AdminServiceClient>();
builder.Services.AddScoped<IAdminCrudService<CategoryViewDtoDto, CategoryEditDtoDto>, AdminCrudService<CategoryViewDtoDto, CategoryEditDtoDto>>();
builder.Services.AddScoped<IAdminCrudService<CrudViewDto, FeatureDtoEditDto>, AdminCrudService<CrudViewDto, FeatureDtoEditDto>>();
builder.Services.AddScoped<IAdminCrudService<CrudViewDto, LookupSpecEditDto>, AdminCrudService<CrudViewDto, LookupSpecEditDto>>();
builder.Services.AddScoped<IAdminCrudService<CrudViewDto, VendorEditDtoDto>, AdminCrudService<CrudViewDto, VendorEditDtoDto>>();
builder.Services.AddScoped<IAdminSeedService, AdminSeedService>();
builder.Services.AddScoped<IAdminCategoryHelper, AdminCategoryHelper>();
builder.Services.AddScoped<IAdminBulkServiceClient, AdminBulkServiceClient>();


builder.Logging.SetMinimumLevel( LogLevel.Error ); // Set the minimum level of logging

await builder.Build().RunAsync();