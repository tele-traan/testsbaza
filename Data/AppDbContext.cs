using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.ApiAuthorization.IdentityServer;

using Duende.IdentityServer.EntityFramework.Options;

using TestsBaza.Models;

namespace TestsBaza.Data
{
#pragma warning disable CS8618
    public class AppDbContext : ApiAuthorizationDbContext<User>
    {
        public AppDbContext(DbContextOptions options, IOptions<OperationalStoreOptions> operationalStoreOptions)
            : base(options, operationalStoreOptions)
        {
        }
        public DbSet<Test> Tests { get; set; }
        public DbSet<Question> Questions { get; set; }
    }
}