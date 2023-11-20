using BlazorElectronics.Shared;
using BlazorElectronics.Shared.Admin.Specs;
using BlazorElectronics.Shared.Admin.Specs.SpecsSingle;

namespace BlazorElectronics.Client.Services.Admin;

public interface IAdminSpecsServiceClient
{
    Task<ApiReply<SpecsViewDto?>> GetSpecsView();
    Task<ApiReply<EditSpecLookupDto?>> AddSpecLookup( AddSpecLookupDto dto );
    Task<ApiReply<bool>> UpdateSpecLookup( EditSpecLookupDto dto );
    Task<ApiReply<bool>> RemoveSpecLookup( RemoveSpecLookupDto dto );
}