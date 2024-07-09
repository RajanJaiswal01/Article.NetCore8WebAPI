using Article.Application.DTO;
using MediatR;

namespace Article.Application.Blog.Command.Create
{
    public class CreateCommand:IRequest<CreateCommandResponse>
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public List<PostDTO>? Posts { get; set; }
    }
}
