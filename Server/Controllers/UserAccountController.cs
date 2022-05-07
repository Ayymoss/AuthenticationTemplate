using BlazorAuthenticationLearn.Server.Services;
using BlazorAuthenticationLearn.Shared;
using Microsoft.AspNetCore.Mvc;

namespace BlazorAuthenticationLearn.Server.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UserAccountController : ControllerBase
{
    private readonly IUserAccountService _userAccountService;

    public UserAccountController(IUserAccountService userAccountService)
    {
        _userAccountService = userAccountService;
    }
    
    [HttpPost]
    public async Task<ActionResult<List<UserAccountDto>>> GetAll() => Ok(await _userAccountService.GetAll());
    
    
}
