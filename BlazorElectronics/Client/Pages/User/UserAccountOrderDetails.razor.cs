using BlazorElectronics.Client.Services.Orders;
using BlazorElectronics.Shared;
using BlazorElectronics.Shared.Orders;
using Microsoft.AspNetCore.Components;

namespace BlazorElectronics.Client.Pages.User;

public sealed partial class UserAccountOrderDetails : UserPage
{
    [Parameter] public string OrderId { get; init; }
    [Inject] public IOrderServiceClient OrderService { get; init; } = default!;

    OrderDetailsDto _details = new();

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();

        if ( !int.TryParse( OrderId, out int id ) )
        {
            SetViewMessage( false, "Invalid url params!" );
            return;
        }

        ServiceReply<OrderDetailsDto?> reply = await OrderService.GetOrderDetails( id );

        if ( !reply.Success || reply.Data is null )
        {
            InvokeAlert( AlertType.Danger, $"Failed to retrieve order details! {reply.ErrorType} : {reply.Message}" );
            return;
        }

        _details = reply.Data;
        StateHasChanged();
    }
}