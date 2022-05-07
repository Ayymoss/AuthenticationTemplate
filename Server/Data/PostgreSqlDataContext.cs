using BlazorAuthenticationLearn.Shared.Models;
using Microsoft.EntityFrameworkCore;

namespace BlazorAuthenticationLearn.Server.Data;

public class PostgreSqlDataContext : DbContext 
{
    public PostgreSqlDataContext(DbContextOptions<PostgreSqlDataContext> options) : base(options)
    {
        
    }
    
    public DbSet<ServerAccountModel> UserAccount { get; set; }

}
