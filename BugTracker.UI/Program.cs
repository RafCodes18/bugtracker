using BugTracker.UI.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Build SQLite connection string.
// Local development uses appsettings.json.
// Azure Linux App Service uses /home/site/data, which is writable/persistent.
var sqliteConnectionString = builder.Configuration.GetConnectionString("BugTrackDb");

if (string.IsNullOrWhiteSpace(sqliteConnectionString))
{
    throw new InvalidOperationException("Connection string 'BugTrackDb' not found.");
}

if (!builder.Environment.IsDevelopment())
{
    var homePath = Environment.GetEnvironmentVariable("HOME") ?? "/home";
    var dataDirectory = Path.Combine(homePath, "site", "data");

    Directory.CreateDirectory(dataDirectory);

    var databasePath = Path.Combine(dataDirectory, "bugtrack.db");

    sqliteConnectionString = $"Data Source={databasePath}";
}

// Add services to the container.
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(sqliteConnectionString));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<IdentityUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = true;
})
.AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddControllersWithViews();

builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(2);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

// Apply migrations and seed demo data on startup.
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

    db.Database.Migrate();

    await DbSeeder.SeedAsync(db);
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

app.Run();