namespace JokesWiki.Models
{
    public class Joke
    {
        public int id { get; set; }
        public string JokeQuestion { get; set; }
        public string JokeAnswer { get; set; }
        public string ApplicationUserID { get; set; }
        public virtual ApplicationUser ApplicationUser { get; set; }
        public string UserEmail { get; set; }
        public Joke()
        {
            
        }
    }
}
