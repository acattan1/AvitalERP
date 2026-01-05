using AvitalERP.Data;
using AvitalERP.Models;
using AvitalERP.Services.Hubspot;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// =======================
// Services
// =======================
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();



// HubSpot (Polling)
builder.Services.Configure<HubspotOptions>(builder.Configuration.GetSection("HubSpot"));

builder.Services.AddHttpClient<HubspotClient>(); // HttpClient tipado

builder.Services.AddScoped<HubspotPollingService>();


// DbContext
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Identity
builder.Services
    .AddDefaultIdentity<AppUser>(options =>
    {
        options.SignIn.RequireConfirmedAccount = false;
    })
    .AddEntityFrameworkStores<AppDbContext>();

var app = builder.Build();

// =======================
// Migraciones + Seeds (dev/prod-safe)
// =======================
using (var scope = app.Services.CreateScope())
{
    try
    {
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        // 1) Migrations existentes (si aplica)
        db.Database.Migrate();

        // 2) Asegura tablas/columnas nuevas en BD existente
        await SchemaBootstrapper.EnsureAsync(db);

        // 3) Seed liviano (catálogos)
        await SeedData.EnsureSeededAsync(db);

        // Admin & roles (si están configurados)
        var adminEmail = builder.Configuration["Admin:Email"];
        var adminPass = builder.Configuration["Admin:Password"];
        if (!string.IsNullOrWhiteSpace(adminEmail) && !string.IsNullOrWhiteSpace(adminPass))
        {
            await IdentitySeed.SeedAsync(app.Services, adminEmail!, adminPass!);
        }
    }
    catch (Exception ex)
    {
        // No bloquear arranque por migración/seed: muestra en consola.
        Console.WriteLine("[Startup] Migración/Seed falló: " + ex.Message);
    }
}

// =======================
// Middleware
// =======================
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

// =======================
// Endpoints
// =======================
app.MapControllers(); // si tienes API controllers
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
