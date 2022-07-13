using BlazorAuthenticationLearn.Server.Data;
using BlazorAuthenticationLearn.Shared.Models;
using BlazorAuthenticationLearn.Shared.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BlazorAuthenticationLearn.Server.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AdminController : ControllerBase
{
    private readonly PostgresqlDataContext _context;
    private readonly UserManager<ApplicationUser> _user;

    public AdminController(PostgresqlDataContext context, UserManager<ApplicationUser> user)
    {
        _context = context;
        _user = user;
    }
    
    [HttpGet]
    [Authorize(Roles = nameof(RoleName.SuperAdmin))]
    public async Task<ActionResult<IList<ApplicationUser>>> GetUser()
    {
        var users = await _context.Users.Select(x => new {x.Id, x.Email, x.UserName}).ToListAsync();
        return Ok(users);
    }
    
    [HttpGet("{id}")]
    [Authorize(Roles = nameof(RoleName.SuperAdmin))]
    public async Task<ActionResult<ApplicationUser>> GetUser(string id)
    {
        var dev = await _context.Users.FirstOrDefaultAsync(a => a.Id == id);
        return Ok(dev);
    }

    [HttpPost]
    [Authorize(Roles = nameof(RoleName.SuperAdmin))]
    public async Task<ActionResult<string>> Post(ApplicationUser user)
    {
        _context.Add(user);
        await _context.SaveChangesAsync();
        return Ok(user.Id);
    }

    [HttpPut]
    [Authorize(Roles = nameof(RoleName.SuperAdmin))]
    public async Task<IActionResult> Put(ApplicationUser user)
    {
        _context.Entry(user).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = nameof(RoleName.SuperAdmin))]
    public async Task<IActionResult> Delete(string id)
    {
        // TODO: Implement user deletion. Simply deleting it as an object isn't possible as we need to log out end user etc.
        return NoContent(); //remove
    }
}
