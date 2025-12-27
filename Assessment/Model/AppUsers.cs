using Microsoft.AspNetCore.Identity;

namespace Assessment.Model
{

    public class ApplicationUser : IdentityUser
    {
        public string? FullName { get; set; }
    }
}
