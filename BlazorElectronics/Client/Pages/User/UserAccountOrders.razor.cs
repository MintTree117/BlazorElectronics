using BlazorElectronics.Client.Services.Orders;
using BlazorElectronics.Shared;
using BlazorElectronics.Shared.Orders;
using Microsoft.AspNetCore.Components;

namespace BlazorElectronics.Client.Pages.User;

public sealed partial class UserAccountOrders : UserPage
{
    [Inject] public IOrderServiceClient OrderService { get; set; } = default!;
    
    List<OrderOverviewDto> _orders = new();
    
    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();

        ServiceReply<List<OrderOverviewDto>?> reply = await OrderService.GetOrders();

        if ( !reply.Success || reply.Data is null )
        {
            SetViewMessage( false, $"Failed to get orders! {reply.ErrorType} : {reply.Message}" );
            return;
        }

        _orders = reply.Data;
        PageIsLoaded = true;
        StateHasChanged();
    }
}