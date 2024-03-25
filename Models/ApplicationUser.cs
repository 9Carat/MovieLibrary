using Microsoft.AspNetCore.Identity;

namespace MovieLibrary.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime BirthDate { get; set; }
        public ICollection<Movie> Movies { get; set; }
    }
}
