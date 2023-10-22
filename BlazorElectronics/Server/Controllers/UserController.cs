using System.Security.Claims;
using BlazorElectronics.Server.Services.Users;
using BlazorElectronics.Shared;
using BlazorElectronics.Shared.Inbound.Users;
using BlazorElectronics.Shared.Outbound.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace BlazorElectronics.Server.Controllers;

[Route( "api/[controller]" )]
[ApiController]
public class UserController : ControllerBase
{
    readonly IUserService _userService;
    
    public UserController( IUserService userService )
    {
        _userService = userService;
    }

    [HttpGet( "test-register" )]
    public async Task<ActionResult<ServiceResponse<int>>> TestRegister()
    {
        var request = new UserRegister_DTO {
            Username = "TestUser",
            Email = "martygrof3708@gmail.com",
            Password = "123456",
            ConfirmPassword = "123456"
        };
        ServiceResponse<int> response = await _userService.RegisterUser( request );

        if ( !response.Success )
            return BadRequest( response );
        return Ok( response );
    }

    [HttpPost( "register" )]
    public async Task<ActionResult<ServiceResponse<int>>> Register( [FromBody] UserRegister_DTO request )
    {
        ServiceResponse<int> response = await _userService.RegisterUser( request );

        if ( !response.Success )
            return BadRequest( response );
        return Ok( response );
    }

    [HttpPost( "login" )]
    public async Task<ActionResult<ServiceResponse<UserLoginResponse_DTO>>> Login( UserLoginRequest_DTO loginRequest )
    {
        ServiceResponse<UserLoginResponse_DTO> response = await _userService.LogUserIn( loginRequest.Email, loginRequest.Password );

        if ( !response.Success )
            return BadRequest( response );
        return Ok( response );
    }

    [HttpPost( "change-password" ), Authorize]
    public async Task<ActionResult<ServiceResponse<bool>>> ChangePassword( [FromBody] string newPassword )
    {
        string? userId = User.FindFirstValue( ClaimTypes.NameIdentifier );

        if ( string.IsNullOrEmpty( userId ) )
            return BadRequest( new ServiceResponse<bool>( false, false, "User doesn't exist!" ) );

        ServiceResponse<bool> response = await _userService.ChangePassword( int.Parse( userId ), newPassword );

        if ( !response.Success )
            return BadRequest( response );
        return Ok( response );
    }
}