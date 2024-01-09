using Blazored.LocalStorage;
using BlazorElectronics.Client.Models;
using BlazorElectronics.Shared;
using BlazorElectronics.Shared.Categories;
using BlazorElectronics.Shared.Enums;

namespace BlazorElectronics.Client.Services.Admin;

public sealed class AdminCategoryHelper : AdminServiceClient, IAdminCategoryHelper
{
    public AdminCategoryHelper( ILogger<ClientService> logger, HttpClient http, ILocalStorageService storage )
        : base( logger, http, storage ) { }

    public List<CategoryViewDtoDto> Categories { get; set; } = new();
    public List<CategorySelectionOption> PrimarySelection { get; set; } = new();

    public async Task<ServiceReply<bool>> Init()
    {
        ServiceReply<List<CategoryViewDtoDto>?> reply = await TryUserGetRequest<List<CategoryViewDtoDto>?>( "api/AdminCategory/get-view" );

        if ( !reply.Success || reply.Payload is null )
        {
            Logger.LogError( reply.ErrorType + reply.Message );
            return new ServiceReply<bool>( reply.ErrorType, reply.Message );
        }

        Categories = reply.Payload;
        PrimarySelection = Categories
            .Where( c => c.Tier == CategoryTier.Primary )
            .Select( c => new CategorySelectionOption( c.Id, c.Name ) )
            .ToList();

        return new ServiceReply<bool>( true );
    }
    public void ResetPrimarySelection()
    {
        PrimarySelection = Categories
            .Where( c => c.Tier == CategoryTier.Primary )
            .Select( c => new CategorySelectionOption( c.Id, c.Name ) )
            .ToList();
    }
    public void SetPrimaryOptions( List<int> modelCategories )
    {
        foreach ( CategorySelectionOption option in PrimarySelection )
        {
            option.IsSelected = modelCategories.Contains( option.Id );
        }
    }
    public List<int> GetSelectedPrimaryOptions()
    {
        return PrimarySelection.Where( c => c.IsSelected )
            .Select( c => c.Id )
            .ToList();
    }
}