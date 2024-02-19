using IdentityServer4.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.CodeAnalysis.Elfie.Extensions;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using UserManagementWithIdentity;
using UserManagementWithIdentity.Data;
using UserManagementWithIdentity.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var usersConnection = builder.Configuration.GetConnectionString("DefaultConnection");
var IdentityConnection = builder.Configuration.GetConnectionString("IdentityConnection");
var migrationsAssembly = typeof(Program).GetTypeInfo().Assembly.GetName().Name;

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(usersConnection, sql => sql.MigrationsAssembly(migrationsAssembly)));



builder.Services.AddDatabaseDeveloperPageExceptionFilter();
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = false )
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultUI()
    .AddDefaultTokenProviders();

builder.Services.AddIdentityServer()
    .AddAspNetIdentity<ApplicationUser>()
    .AddTestUsers(Config.Users)

    /*
    .AddInMemoryClients(Config.Clients)
    .AddInMemoryIdentityResources(Config.IdentityResources)
    .AddInMemoryApiResources(Config.ApiResources)
    .AddInMemoryApiScopes(Config.ApiScopes)
    */
    .AddConfigurationStore(option => {
        option.ConfigureDbContext = builder => builder.UseSqlServer(IdentityConnection, sql => sql.MigrationsAssembly(migrationsAssembly) );

    
    })
    .AddOperationalStore(option => {
        option.ConfigureDbContext = builder => builder.UseSqlServer(IdentityConnection , sql => sql.MigrationsAssembly(migrationsAssembly));


    })
    .AddDeveloperSigningCredential();


builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseIdentityServer();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();
