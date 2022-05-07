using BlazorAuthenticationLearn.Shared;
using Microsoft.EntityFrameworkCore;

namespace BlazorAuthenticationLearn.Server.Data;

public class PostgreSqlDataContext : DbContext 
{
    public PostgreSqlDataContext(DbContextOptions<PostgreSqlDataContext> options) : base(options)
    {
        
    }
    
    public DbSet<UserAccountDto> UserAccount { get; set; }

}
