using AvitalERP.Data;
using AvitalERP.Models;
using AvitalERP.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// 1) EF Core + SQL Server (OBLIGATORIO antes de Identity)
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// 2) Identity + Roles
builder.Services
    .AddIdentity<AppUser, IdentityRole>(options =>
    {
        options.SignIn.RequireConfirmedAccount = false;

        options.Password.RequireDigit = true;
        options.Password.RequireNonAlphanumeric = true;
        options.Password.RequireUppercase = true;
        options.Password.RequireLowercase = true;
        options.Password.RequiredLength = 10;
    })
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders()
    .AddDefaultUI();

builder.Services.AddAuthorization();

// 3) Servicios personalizados
builder.Services.AddScoped<IXmlImportService, XmlImportService>();

// 4) Blazor Server
builder.Services.AddRazorPages();        // necesario para /Identity/*
builder.Services.AddServerSideBlazor();

var app = builder.Build();

// (Opcional pero recomendado) Aplicar migraciones automáticamente al arrancar
// Si prefieres NO hacerlo, puedes quitar este bloque.
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await db.Database.MigrateAsync();
}

// Seed roles + admin
await IdentitySeed.SeedAsync(app.Services,
    adminEmail: builder.Configuration["SeedAdmin:Email"] ?? "abraham@avital.mx",
    adminPassword: builder.Configuration["SeedAdmin:Password"] ?? "Avital!23456"
);

// Pipeline
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

app.MapRazorPages();
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
