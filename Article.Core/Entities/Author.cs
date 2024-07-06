using Article.Core.Common;

namespace Article.Core.Entities
{
    public class Author:BaseEntity
    {
        public long Id { get; set; }
        public string? Designation { get; set; }
        public string? Category { get; set; }
        public string? UserId { get; set; }
        public virtual ICollection<Post> Posts { get; set; }
        public virtual User User { get; set; }

    }
}
