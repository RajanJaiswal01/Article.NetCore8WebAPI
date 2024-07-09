using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Article.Application.DTO
{
    public class PostDTO
    {
        public long Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; } = false;
        public long AuthorId { get; set; }
        public long BlogId { get; set; }
        public long AuthorName { get; set; }
    }
}
