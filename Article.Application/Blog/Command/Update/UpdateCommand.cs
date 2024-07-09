using Article.Application.DTO;
using MediatR;

namespace Article.Application.Blog.Command.Update
{
    public class UpdateCommand:IRequest<UpdateCommandResponse>
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
        public List<PostDTO> Posts { get; set; }

    }
}
