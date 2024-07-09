using MediatR;

namespace Article.Application.Blog.Query.GetById
{
    public class GetByIdQuery : IRequest<GetByIdQueryResponse>
    {
        public long Id { get; set; }
    }
}
