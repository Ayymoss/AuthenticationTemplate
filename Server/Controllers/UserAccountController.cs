using BlazorAuthenticationLearn.Shared.Models;
using BlazorAuthenticationLearn.Server.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BlazorAuthenticationLearn.Server.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UserAccountController : ControllerBase
{
    private readonly PostgreSqlDataContext _context;
    
    public UserAccountController(PostgreSqlDataContext context)
    {
        _context = context;
    }

    [HttpPost("Login")]
    public async Task<ActionResult<ClientGlobalAccountModel>> Login(ClientLoginAccountModel user)
    {
        var userExists = await _context.UserAccount
            .FirstOrDefaultAsync(x => x.Username == user.Username);

        if (userExists == null)
        {
            return BadRequest("Username or password incorrect or account doesn't exist.");
        }

        var (hash, salt) = PasswordHashing.HashPassword(user.Password,
            Convert.FromBase64String(userExists.PasswordSalt));

        if (userExists.PasswordHash != hash)
        {
            return BadRequest("Username or password incorrect or account doesn't exist.");
        }
        
        var globalAccountModel = new ClientGlobalAccountModel
        {
            Id = userExists.Id,
            Username = userExists.Username
        };

        return Ok(globalAccountModel);
    }

    [HttpPost("Register")]
    public async Task<ActionResult<ClientGlobalAccountModel>> CreateUser(ClientRegisterAccountModel user)
    {
        var userExists = await _context.UserAccount
            .FirstOrDefaultAsync(x => x.Username == user.Username || x.Email == user.Email);

        if (userExists != null)
        {
            return BadRequest("Username or Email already exists");
        }

        var (hash, salt) = PasswordHashing.HashPassword(user.Password);
        var serverAccountModel = new ServerAccountModel
        {
            Email = user.Email,
            Username = user.Username,
            PasswordHash = hash,
            PasswordSalt = Convert.ToBase64String(salt)
        };

        _context.UserAccount.Add(serverAccountModel);
        await _context.SaveChangesAsync();

        var globalAccountModel = new ClientGlobalAccountModel
        {
            Id = serverAccountModel.Id,
            Username = serverAccountModel.Username
        };

        return Ok(globalAccountModel);
    }
}
