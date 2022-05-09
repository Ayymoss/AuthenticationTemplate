using BlazorAuthenticationLearn.Server;
using BlazorAuthenticationLearn.Server.Data;
using BlazorAuthenticationLearn.Server.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

// TODO: Add Password Reset form

var databaseEnvironment = Environment.GetEnvironmentVariable("PostgreSQLDBConnection");

if (databaseEnvironment == null)
{
    DatabaseConfiguration.DatabaseInit();
}

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

// Custom Services
builder.Services.AddDbContext<PostgresqlDataContext>(options => { options.UseNpgsql(databaseEnvironment); });

builder.Services.AddIdentity<ApplicationUser, IdentityRole>().AddEntityFrameworkStores<PostgresqlDataContext>();
builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.HttpOnly = false;
    options.Events.OnRedirectToLogin = context =>
    {
        context.Response.StatusCode = 401;
        return Task.CompletedTask;
    };
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Generated app builder using above services.
var app = builder.Build();

// Add SwaggerUI
app.UseSwaggerUI();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

// Use Swagger
app.UseSwagger();

app.UseHttpsRedirection();

app.UseBlazorFrameworkFiles();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();
app.MapControllers();
app.MapFallbackToFile("index.html");

app.Run();
