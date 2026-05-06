using Microsoft.AspNetCore.Identity;

namespace Workshop.Models
{
    public class User : IdentityUser
    {
        public string FullName { get; set; }
        public string ProfileImagePath { get; set; }
    }
}