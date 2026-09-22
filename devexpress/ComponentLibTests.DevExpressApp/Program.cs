using ComponentLibTests.Data;
using ComponentLibTests.DevExpressApp.Components;
using ComponentLibTests.Domain.Entities;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddCascadingAuthenticationState();

builder.Services.AddDevExpressBlazor();

builder.Services.AddDbContextFactory<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
// Identity's UserManager/SignInManager need a directly-injectable scoped AppDbContext (the factory above doesn't register one).
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddIdentity<ApplicationUser, IdentityRole<Guid>>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/login";
    options.AccessDeniedPath = "/access-denied";
});

builder.Services.AddAuthorization();
builder.Services.AddScoped<AuthenticationStateProvider, ServerAuthenticationStateProvider>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseAuthentication();
app.UseAuthorization();
app.UseAntiforgery();

app.MapPost("/account/login", async (
    [FromForm] string username,
    [FromForm] string password,
    [FromQuery] string? returnUrl,
    SignInManager<ApplicationUser> signInManager,
    [FromForm] bool rememberMe = false) =>
{
    var result = await signInManager.PasswordSignInAsync(username, password, rememberMe, lockoutOnFailure: false);

    if (result.Succeeded)
    {
        return Results.LocalRedirect(string.IsNullOrEmpty(returnUrl) ? "/" : returnUrl);
    }

    var loginUrl = $"/login?error={Uri.EscapeDataString("Invalid username or password.")}";
    if (!string.IsNullOrEmpty(returnUrl))
    {
        loginUrl += $"&returnUrl={Uri.EscapeDataString(returnUrl)}";
    }
    return Results.Redirect(loginUrl);
}).DisableAntiforgery();

app.MapPost("/account/logout", async (SignInManager<ApplicationUser> signInManager) =>
{
    await signInManager.SignOutAsync();
    return Results.LocalRedirect("/login");
}).DisableAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

await using (var scope = app.Services.CreateAsyncScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await db.Database.MigrateAsync();

    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
    const string adminUserName = "admin";
    if (await userManager.FindByNameAsync(adminUserName) is null)
    {
        var admin = new ApplicationUser { UserName = adminUserName, DisplayName = "Administrator" };
        var result = await userManager.CreateAsync(admin, "Admin123!");
        if (!result.Succeeded)
        {
            throw new InvalidOperationException(
                $"Failed to seed default admin user: {string.Join(", ", result.Errors.Select(e => e.Description))}");
        }
    }
}

app.Run();
