using BlazorElectronics.Client.Models;
using BlazorElectronics.Client.Services.Admin;
using BlazorElectronics.Shared;
using BlazorElectronics.Shared.Products;
using Microsoft.AspNetCore.Components;

namespace BlazorElectronics.Client.Pages.Admin.BulkInsert;

public partial class BulkProductKeys : AdminPage
{
    [Inject] public IAdminBulkServiceClient BulkService { get; set; } = default!;
    readonly ProductKeySeed _seed = new();

    async Task Submit()
    {
        var keys = new ProductKeysDto
        {
            ProductId = _seed.ProductId,
            Keys = _seed.Keys
                .Split( "," )
                .ToList()
        };

        ServiceReply<bool> reply = await BulkService.BulkInsertKeys( keys );

        if ( !reply.Success )
        {
            Logger.LogError( reply.ErrorType + reply.Message );
            //SetActionMessage( false, reply.ErrorType + reply.Message );
            return;
        }

        //SetActionMessage( true, "Successfully inserted product keys." );
    }
}