using BlazorElectronics.Client.Services.Admin;
using BlazorElectronics.Shared.Admin.Specs.SpecsSingle;
using Microsoft.AspNetCore.Components;

namespace BlazorElectronics.Client.Pages.UserViews.Admin;

public sealed partial class AdminSpecsEdit : AdminView
{
    [Inject] IAdminSpecsServiceClient AdminSpecService { get; set; } = default!;
    
    EditSpecLookupDto _dto = new();
    
    bool _newSpec;
    
    async Task Submit()
    {
        
    }
}