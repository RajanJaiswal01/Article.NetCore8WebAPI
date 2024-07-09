using Article.Application.Blog.Query.GetById;
using Article.Application.CustomExceptions;
using Article.Application.DTO;
using Article.Core.IReposiories;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Article.Application.Blog.Query.GetAll
{
    public class GetAllQueryHandler:IRequestHandler<GetAllQuery, GetAllQueryResponse>
    {
        private readonly ILogger<GetAllQueryHandler> _logger;
        private readonly IBlogRepository _blogRepository;
        private readonly IMapper _mapper;

        public GetAllQueryHandler(ILogger<GetAllQueryHandler> logger, IBlogRepository blogRepository, IMapper mapper)
        {
            _logger = logger;
            _blogRepository = blogRepository;
            _mapper = mapper;
        }
        public async Task<GetAllQueryResponse> Handle(GetAllQuery request, CancellationToken cancellationToken)
        {
            var response = new GetAllQueryResponse();

            var BlogData = await _blogRepository.GetAll(x => x.IsDeleted != true, x => x.Posts);
            if (BlogData.Count() == 0)
            {
                response.IsError = true;
                throw new NotFoundException($"Data Not Found for Blog");
            }
            else
            {
                List<BlogDTO> blogDTO = _mapper.Map<List<BlogDTO>>(BlogData);
                response.IsError = false;
                response.Data = blogDTO;
                return response;
            }
        }
    }
}
