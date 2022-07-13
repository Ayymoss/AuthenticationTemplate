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
    
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        
        // Build seed for role SuperAdmin
        var superAdminRole = new IdentityRole
        {
            Id = Guid.NewGuid().ToString(),
            Name = "SuperAdmin",
            NormalizedName = "SUPERADMIN",
            ConcurrencyStamp = Guid.NewGuid().ToString()
        };
        
        var youTubeRole = new IdentityRole
        {
            Id = Guid.NewGuid().ToString(),
            Name = "YouTube",
            NormalizedName = "YOUTUBE",
            ConcurrencyStamp = Guid.NewGuid().ToString()
        };

        // Build seed for user SuperAdmin
        var superAdminUser = new ApplicationUser()
        {
            Id = Guid.NewGuid().ToString(),
            UserName = "SuperAdmin",
            NormalizedUserName = "SUPERADMIN",
            Email = "email@example.org",
            NormalizedEmail = "EMAIL@EXAMPLE.ORG",
            EmailConfirmed = true,
            PhoneNumberConfirmed = false,
            TwoFactorEnabled = false,
            LockoutEnabled = false,
            ConcurrencyStamp = Guid.NewGuid().ToString(),
            SecurityStamp = Guid.NewGuid().ToString()
        };

        // Generate password hash for user SuperAdmin
        var passwordHasher = new PasswordHasher<ApplicationUser>();
        superAdminUser.PasswordHash = passwordHasher.HashPassword(superAdminUser, "adminsuper");

        // Seed Role and User
        builder.Entity<IdentityRole>().HasData(superAdminRole);
        builder.Entity<IdentityRole>().HasData(youTubeRole);
        builder.Entity<ApplicationUser>().HasData(superAdminUser);

        // Set user SuperAdmin to role SuperAdmin
        builder.Entity<IdentityUserRole<string>>().HasData(new IdentityUserRole<string>
        {
            
            RoleId = superAdminRole.Id,
            UserId = superAdminUser.Id
        });
    }
}
