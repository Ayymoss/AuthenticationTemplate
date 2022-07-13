using BlazorAuthenticationLearn.Server;
using BlazorAuthenticationLearn.Server.Data;
using BlazorAuthenticationLearn.Server.Utilities;
using BlazorAuthenticationLearn.Shared.Models;
using Blazored.Toast;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

// TODO: Add Admin Area for Password Resets and User management (Deletions etc)

// TODO: File Search
// TODO: Change the "Trash" icon next to uploads.

// TODO: Fill site with toasts.

// DONE: File download and delete need user check (renamed as not sure if I want to keep this)
// DONE: If I do allow file deletions/downloads from other users, the path needs to be pulled from the DB user too

SetupConfiguration.InitConfiguration();
var configuration = SetupConfiguration.ReadConfiguration();

var builder = WebApplication.CreateBuilder(args);

#if !DEBUG
builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(5000);
    options.ListenAnyIP(5001, configure => configure.UseHttps());
});
#endif

#if DEBUG
builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenLocalhost(5000);
    options.ListenLocalhost(5001, configure => configure.UseHttps());
});
#endif

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

// Custom Services
builder.Services.AddSingleton(configuration);

builder.Services.AddDbContext<PostgresqlDataContext>(
    options =>
    {
        options.UseNpgsql($"Host={configuration.Database.HostName};" +
                          $"Port={configuration.Database.Port};" +
                          $"Username={configuration.Database.UserName};" +
                          $"Password={configuration.Database.Password};" +
                          $"Database={configuration.Database.Database}");
    });

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

builder.Services.Configure<FormOptions>(x =>
{
    x.ValueLengthLimit = int.MaxValue;
    x.MultipartBodyLengthLimit = int.MaxValue;
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddBlazoredToast();
builder.Services.AddHttpClient();

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
