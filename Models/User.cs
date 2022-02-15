using Microsoft.AspNetCore.Identity;

namespace TestsBaza.Models
{
    public class User : IdentityUser
    {
#pragma warning disable CS8618
        public IEnumerable<Test> Tests { get; set; }
        public 
    }
}