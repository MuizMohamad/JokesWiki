using Microsoft.AspNetCore.Identity;

namespace JokesWiki.Models
{
    public class ApplicationUser : IdentityUser
    {
        public virtual ICollection<Joke> Jokes { get; set; }

    }
}
