global using System.Linq;
global using System.Threading.Tasks;
global using System.Collections.Generic;

global using TestsBaza.Models;



using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.AspNetCore.Authentication;

using TestsBaza.Data;
using TestsBaza.Repositories;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

IServiceCollection services = builder.Services;
{
    string connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

    services.AddDbContext<AppDbContext>(options =>
        options.UseSqlServer(connectionString));
    services.AddDatabaseDeveloperPageExceptionFilter();

    services.AddDefaultIdentity<User>(options => {
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequiredLength = 5;
        options.Password.RequireDigit = false;
        options.Password.RequireLowercase = false;
        options.Password.RequireUppercase = false;
        options.SignIn.RequireConfirmedAccount = true;
        })
        .AddEntityFrameworkStores<AppDbContext>();

    services.AddIdentityServer()
        .AddApiAuthorization<User, AppDbContext>();

    services.AddAuthentication()
        .AddIdentityServerJwt();

    services.AddControllersWithViews();
    services.AddRazorPages();

    services.AddTransient<ITestsRepository, TestsRepository>();
}

WebApplication app = builder.Build();
{
    if (app.Environment.IsDevelopment()) app.UseMigrationsEndPoint();
    else app.UseHsts();
    

    app.UseHttpsRedirection();
    app.UseStaticFiles();
    app.UseRouting();

    app.UseAuthentication();
    app.UseIdentityServer();
    app.UseAuthorization();

    app.MapControllerRoute(
        name: "default",
        pattern: "{controller}/{action=Index}/{id?}");
    app.MapRazorPages();

    app.MapFallbackToFile("index.html"); ;

}

app.Run();