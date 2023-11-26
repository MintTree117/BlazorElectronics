using BlazorElectronics.Server.Admin.Repositories;
using BlazorElectronics.Server.Services.Sessions;
using BlazorElectronics.Server.Services.Users;
using BlazorElectronics.Shared.Admin;
using BlazorElectronics.Shared.Admin.Products;
using Microsoft.AspNetCore.Mvc;

namespace BlazorElectronics.Server.Admin.Controllers;

[Route( "api/[controller]" )]
[ApiController]
public sealed class AdminProductController : _AdminController
{
    readonly IAdminProductRepository _repository;
    
    public AdminProductController( ILogger<AdminProductController> logger, IUserAccountService userAccountService, ISessionService sessionService, IAdminProductRepository repository )
        : base( logger, userAccountService, sessionService )
    {
        _repository = repository;
    }
}