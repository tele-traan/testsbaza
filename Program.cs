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
using Microsoft.AspNetCore.Authentication.Cookies;

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
        options.SignIn.RequireConfirmedAccount = false;
        
        })
        .AddEntityFrameworkStores<AppDbContext>();

    services.AddIdentityServer()
        .AddApiAuthorization<User, AppDbContext>();

    services.AddAuthentication()
        .AddIdentityServerJwt()
        .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme);

    services.AddControllersWithViews().ConfigureApiBehaviorOptions(options => options.SuppressModelStateInvalidFilter = true);
    services.AddRazorPages();
    services.AddCors();
    services.AddTransient<ITestsRepository, TestsRepository>();
}

WebApplication app = builder.Build();
{
    if (app.Environment.IsDevelopment()) app.UseMigrationsEndPoint();
    else app.UseHsts();

    app.UseHttpsRedirection();
    app.UseStaticFiles();

    app.UseCors(options => options.AllowAnyHeader().AllowAnyMethod().AllowCredentials());
    app.UseRouting();

    app.UseAuthentication();
    app.UseIdentityServer();
    app.UseAuthorization();

    app.MapControllerRoute(
        name: "default",
        pattern: "{controller}/{action=Index}/{id?}");
    app.MapRazorPages();
    app.MapControllers();
    app.MapFallbackToFile("index.html");

}

app.Run();