using Microsoft.AspNetCore.Identity;

namespace TestsBaza.Models
{
    public class User : IdentityUser
    {
        public IEnumerable<Test> Tests { get; set; } = new List<Test>();
    }
}