using BlazorAuthenticationLearn.Server.Data;
using BlazorAuthenticationLearn.Shared.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BlazorAuthenticationLearn.Server.Controllers;

[Route("api/[controller]")]
[ApiController]
public class DeveloperController : ControllerBase
{
    private readonly PostgresqlDataContext _context;

    public DeveloperController(PostgresqlDataContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var devs = await _context.Developers.ToListAsync();
        return Ok(devs);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Get(int id)
    {
        var dev = await _context.Developers.FirstOrDefaultAsync(a => a.Id == id);
        return Ok(dev);
    }

    [HttpPost]
    public async Task<IActionResult> Post(Developer developer)
    {
        _context.Add(developer);
        await _context.SaveChangesAsync();
        return Ok(developer.Id);
    }

    [HttpPut]
    public async Task<IActionResult> Put(Developer developer)
    {
        _context.Entry(developer).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var dev = new Developer {Id = id};
        _context.Remove(dev);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}
