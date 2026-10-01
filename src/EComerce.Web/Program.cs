using EComerce.Application;
using EComerce.Application.Interfaces;
using EComerce.Infrastructure;
using EComerce.Infrastructure.Identity;
using EComerce.Infrastructure.Persistence;
using EComerce.Web.Data;
using EComerce.Web.Hubs;
using EComerce.Web.Middleware;
using EComerce.Web.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// ── Capas ─────────────────────────────────────────────────────────────────────
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

// ── Identity (registrado en Web por depender de ASP.NET Core) ─────────────────
// Se usa AddDefaultIdentity (compatible con las Identity UI Razor Pages del template)
// + AddRoles para soporte de roles.
builder.Services.AddDefaultIdentity<ApplicationUser>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 8;

    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
    options.Lockout.MaxFailedAccessAttempts = 5;

    options.User.RequireUniqueEmail = true;
})
.AddRoles<IdentityRole>()
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

// ── Web ────────────────────────────────────────────────────────────────────────
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages(); // Para las páginas de Identity scaffolded

// SignalR
builder.Services.AddSignalR();

// Cookie de Identity: redirige a login en lugar de 401 bare en MVC
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.AccessDeniedPath = "/Account/AccessDenied";
});

var app = builder.Build();

// ── Middleware de errores (debe ir primero) ────────────────────────────────────
app.UseGlobalExceptionHandler();

// ── Pipeline HTTP ──────────────────────────────────────────────────────────────
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseHsts();
}

app.UseHttpsRedirection();
app.MapStaticAssets();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

// ── Rutas ──────────────────────────────────────────────────────────────────────
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.MapRazorPages()
   .WithStaticAssets();

// Hub de chat SignalR
app.MapHub<ChatHub>("/hubs/chat");

// ── Migraciones y seed automáticos ────────────────────────────────────────────
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    await db.Database.MigrateAsync();
}

await DatabaseSeeder.SeedAsync(app.Services);

app.Run();
