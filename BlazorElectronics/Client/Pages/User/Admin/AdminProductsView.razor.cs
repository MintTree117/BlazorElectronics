using Microsoft.AspNetCore.Components;

namespace BlazorElectronics.Client.Pages.User.Admin;

public partial class AdminProductsView : AdminView
{
    [Parameter] public string? Primary { get; set; }
    [Parameter] public string? Secondary { get; set; }
    [Parameter] public string? Tertiary { get; set; }
    // search by name
    // search by id
    // list of products
    // pagination buttons at the bottom
    // display id, image, name, lowest and highest price
}