namespace Article.Application.DTO
{
    public class BlogDTO
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; } = false;
        public List<PostDTO> Posts { get; set; }
    }
}
