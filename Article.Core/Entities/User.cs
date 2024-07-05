using Microsoft.AspNetCore.Identity;

namespace Article.Core.Entities
{
    public class User:IdentityUser
    {
        public string? Nationality { get; set; }
        public Address? Address {  get; set; } 
        public virtual ICollection<Author> Authors { get; set; }
    }
}
