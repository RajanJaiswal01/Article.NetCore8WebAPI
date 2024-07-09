using Article.Application.CustomExceptions;
using Article.Application.DTO;
using Article.Core.IReposiories;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Article.Application.Blog.Query.GetById
{
    public class GetByIdQueryHandler : IRequestHandler<GetByIdQuery, GetByIdQueryResponse>
    {
        private readonly ILogger<GetByIdQueryHandler> _logger;
        private readonly IBlogRepository _blogRepository;
        private readonly IMapper _mapper;

        public GetByIdQueryHandler(ILogger<GetByIdQueryHandler> logger, IBlogRepository blogRepository, IMapper mapper)
        {
            _logger = logger;
            _blogRepository = blogRepository;
            _mapper = mapper;
        }
        public async Task<GetByIdQueryResponse> Handle(GetByIdQuery request, CancellationToken cancellationToken)
        {
            var response = new GetByIdQueryResponse();
            var validator = new GetByIdQueryValidator();

            var validation = await validator.ValidateAsync(request, cancellationToken);

            if (validation.Errors.Count() > 0)
            {
                response.IsError = true;
                response.Id = request.Id;
                _logger.LogError($"Id : {request.Id} Validation Error.");
                foreach (var error in validation.Errors.Select(x => x.ErrorMessage))
                {
                    response.ErrorsList.Add(error);
                }
                return response;
            }
            else
            {
                var BlogData = await _blogRepository.GetAll(x => x.Id == request.Id && x.IsDeleted != true, x => x.Posts);
                var blogData = BlogData.FirstOrDefault();
                if (blogData == null)
                {
                    response.IsError = true;
                    response.Id = request.Id;
                    throw new NotFoundException($"Data Not Found for BlogId : {request.Id}");
                }
                else
                {
                    BlogDTO blogDTO = _mapper.Map<BlogDTO>(blogData);
                    response.IsError = false;
                    response.Data = blogDTO;
                    return response;
                }
            }
        }
    }
}
