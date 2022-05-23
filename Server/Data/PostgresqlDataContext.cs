using BlazorAuthenticationLearn.Server.Models;
using BlazorAuthenticationLearn.Shared.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BlazorAuthenticationLearn.Server.Data;

public class PostgresqlDataContext : IdentityDbContext<ApplicationUser>
{
    public PostgresqlDataContext(DbContextOptions<PostgresqlDataContext> options) : base(options)
    {
    }

    public DbSet<Developer> Developers { get; set; }
    public DbSet<FileContext> FileContexts { get; set; }

    private readonly string _roleGuid = Guid.NewGuid().ToString();
    private readonly string _roleGuidConStamp = Guid.NewGuid().ToString();
    private readonly string _userGuid = Guid.NewGuid().ToString();
    private readonly string _userGuidConStamp = Guid.NewGuid().ToString();
    private readonly string _userGuidSecStamp = Guid.NewGuid().ToString();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        
        // Build seed for role SuperAdmin
        var role = new IdentityRole
        {
            Id = _roleGuid,
            Name = "SuperAdmin",
            NormalizedName = "SUPERADMIN",
            ConcurrencyStamp = _roleGuidConStamp
        };

        // Build seed for user SuperAdmin
        var user = new ApplicationUser()
        {
            Id = _userGuid,
            UserName = "SuperAdmin",
            NormalizedUserName = "SUPERADMIN",
            Email = "email@example.org",
            NormalizedEmail = "EMAIL@EXAMPLE.ORG",
            EmailConfirmed = true,
            PhoneNumberConfirmed = false,
            TwoFactorEnabled = false,
            LockoutEnabled = false,
            ConcurrencyStamp = _userGuidConStamp,
            SecurityStamp = _userGuidSecStamp
        };

        // Generate password hash for user SuperAdmin
        var passwordHasher = new PasswordHasher<ApplicationUser>();
        user.PasswordHash = passwordHasher.HashPassword(user, "adminsuper");

        // Seed Role and User
        builder.Entity<IdentityRole>().HasData(role);
        builder.Entity<ApplicationUser>().HasData(user);

        // Set user SuperAdmin to role SuperAdmin
        builder.Entity<IdentityUserRole<string>>().HasData(new IdentityUserRole<string>
        {
            
            RoleId = _roleGuid,
            UserId = _userGuid
        });
    }
}
