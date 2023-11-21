using BlazorElectronics.Client.Services.Users.Admin;
using BlazorElectronics.Shared.Admin.Specs;
using Microsoft.AspNetCore.Components;

namespace BlazorElectronics.Client.Pages.User.Admin;

public sealed partial class AdminSpecsEdit : AdminView
{
    [Inject] IAdminSpecsServiceClient AdminSpecService { get; set; } = default!;
    
    EditSpecLookupDto _dto = new();
    
    bool _newSpec;
    
    async Task Submit()
    {
        
    }
}