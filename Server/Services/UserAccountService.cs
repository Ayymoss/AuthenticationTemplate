using BlazorAuthenticationLearn.Server.Data;
using BlazorAuthenticationLearn.Shared;
using Microsoft.EntityFrameworkCore;

namespace BlazorAuthenticationLearn.Server.Services;

public class UserAccountService : IUserAccountService
{
    private readonly PostgreSqlDataContext _context;

    public UserAccountService(PostgreSqlDataContext context)
    {
        _context = context;
    }

    public async Task<List<UserAccountDto>> GetAll() => await _context.UserAccount.ToListAsync();
    
}
