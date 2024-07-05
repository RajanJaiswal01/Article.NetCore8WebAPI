using Article.Core.Common;

namespace Article.Core.Entities
{
    public class Blog : BaseEntity
    {
        public long Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public virtual ICollection<Posts> Posts { get; set; }
    }
}
