using BlazorElectronics.Client.Services.Admin;
using BlazorElectronics.Shared;
using Microsoft.AspNetCore.Components;

namespace BlazorElectronics.Client.Pages.Admin.TestSeed;

public partial class TestSeedPage : AdminPage
{
    const string FAIL_SEED_MESSAGE = "Failed to seed items!";
    
    [Inject] IAdminSeedService SeedService { get; init; } = default!;

    readonly IntDto ProductCount = new();
    readonly IntDto UserCount = new();

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();

        if ( !PageIsAuthorized )
            return;

        PageIsLoaded = true;
    }
    
    async Task SeedProducts()
    {
        ServiceReply<bool> reply = await SeedService.SeedProducts( ProductCount );
        InvokeAlert( reply.Success ? AlertType.Success : AlertType.Danger, reply.Success ? "Successfully seeded products." : reply.Message ?? FAIL_SEED_MESSAGE );
    }

    async Task SeedUsers()
    {
        Logger.LogError( "hit" );
        ServiceReply<bool> reply = await SeedService.SeedUsers( UserCount );
        InvokeAlert( reply.Success ? AlertType.Success : AlertType.Danger, reply.Success ? "Successfully seeded users." : reply.Message ?? FAIL_SEED_MESSAGE );
    }
}