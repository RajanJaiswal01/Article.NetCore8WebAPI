using Article.Core.Common;

namespace Article.Core.Entities
{
    public class Post:BaseEntity
    {
        public long Id { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public long? AuthorId { get; set; }
        public long? BlogId { get; set; }
        public virtual Blog? Blog { get; set; }
        public virtual Author? Author { get; set; }
    }
}
