using BlazorElectronics.Client.Services.Admin;
using BlazorElectronics.Client.Services.Categories;
using BlazorElectronics.Client.Services.Specs;
using BlazorElectronics.Client.Services.Vendors;
using BlazorElectronics.Shared;
using BlazorElectronics.Shared.Categories;
using BlazorElectronics.Shared.Products;
using BlazorElectronics.Shared.Specs;
using BlazorElectronics.Shared.Vendors;
using Microsoft.AspNetCore.Components;

namespace BlazorElectronics.Client.Pages.Admin.ProductEdit;

public partial class EditProduct : AdminPage
{
    const string API_PATH = "api/AdminProduct";
    const string API_PATH_GET_EDIT = $"{API_PATH}/get-edit";
    const string API_PATH_ADD = $"{API_PATH}/add";
    const string API_PATH_EDIT = $"{API_PATH}/edit";
    
    [Inject] public IAdminCrudService<CrudViewDto, ProductEditDto> ProductService { get; set; } = default!;
    [Inject] public IVendorServiceClient VendorService { get; set; } = default!;
    [Inject] public ICategoryServiceClient CategoryService { get; set; } = default!;
    [Inject] public ISpecServiceClient SpecService { get; set; } = default!;
    
    [Parameter] public string ProductId { get; set; } = string.Empty;

    ProductEditDto _productDto = new();
    bool _isNew = false;

    VendorsDto _vendors = default!;
    CategoryData _categories = default!;
    LookupSpecsDto _specs = default!;
    
    protected override async Task OnInitializedAsync()
    {
        if ( !ParseUrl( out int id ) )
            return;

        ServiceReply<ProductEditDto?> loadReply = await ProductService.GetEdit( API_PATH_GET_EDIT, id );
        PageIsLoaded = true;
        
        if ( !loadReply.Success || loadReply.Data is null )
        {
            SetViewMessage( false, $"Failed to load product! {loadReply.ErrorType} : {loadReply.Message}" );
            return;
        }

        _productDto = loadReply.Data;
    }

    bool ParseUrl( out int id )
    {
        Uri uri = NavManager.ToAbsoluteUri( NavManager.Uri );
        Dictionary<string, string> queryParams = ParseQuery( uri.Query );
        queryParams.TryGetValue( Routes.PRODUCT_EDIT_NEW_PARAM, out string? newProduct );
        bool.TryParse( newProduct, out _isNew );
        
        if ( int.TryParse( ProductId, out id ) )
            return true;

        SetViewMessage( false, "Invalid Url!" );
        return false;
    }
    
    async Task LoadVendors()
    {
        var reply = await VendorService.GetVendors();
    }
    async Task LoadCategories() { }
    async Task LoadSpecs() { }

    async Task Submit()
    {
        if ( _isNew )
            await SubmitNew();
        else
            await SubmitEdit();
    }
    async Task SubmitNew()
    {
        ServiceReply<int> reply = await ProductService.Add( API_PATH_ADD, _productDto );

        if ( !reply.Success )
        {
            InvokeAlert( AlertType.Danger, $"Failed to submit new product! {reply.ErrorType} : {reply.Message}" );
            return;
        }

        _isNew = false;
        _productDto.ProductId = reply.Data;
        StateHasChanged();
    }
    async Task SubmitEdit()
    {
        ServiceReply<int> reply = await ProductService.Add( API_PATH_EDIT, _productDto );

        if ( !reply.Success )
        {
            InvokeAlert( AlertType.Danger, $"Failed to submit changes! {reply.ErrorType} : {reply.Message}" );
            return;
        }
        
        StateHasChanged();
    }
}