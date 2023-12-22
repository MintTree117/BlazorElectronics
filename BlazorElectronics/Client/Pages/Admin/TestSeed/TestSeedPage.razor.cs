using BlazorElectronics.Client.Services.Admin;
using BlazorElectronics.Shared;
using Microsoft.AspNetCore.Components;

namespace BlazorElectronics.Client.Pages.Admin.TestSeed;

public partial class TestSeedPage : AdminPage
{
    const string FAIL_SEED_MESSAGE = "Failed to seed items!";
    
    [Inject] IAdminSeedService SeedService { get; init; } = default!;

    readonly IntDto ProductCount = new();
    readonly IntDto ReviewCount = new();
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
        ServiceReply<bool> reply = await SeedService.SeedProducts( ProductCount.Value );
        InvokeAlert( reply.Success ? AlertType.Success : AlertType.Danger, reply.Success ? "Successfully seeded products." : reply.Message ?? FAIL_SEED_MESSAGE );
    }
    async Task SeedReviews()
    {
        ServiceReply<bool> reply = await SeedService.SeedReviews( ReviewCount.Value );
        InvokeAlert( reply.Success ? AlertType.Success : AlertType.Danger, reply.Success ? "Successfully seeded reviews." : reply.Message ?? FAIL_SEED_MESSAGE );
    }
    async Task SeedUsers()
    {
        ServiceReply<bool> reply = await SeedService.SeedUsers( UserCount.Value );
        InvokeAlert( reply.Success ? AlertType.Success : AlertType.Danger, reply.Success ? "Successfully seeded users." : reply.Message ?? FAIL_SEED_MESSAGE );
    }
}