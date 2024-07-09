using Article.Core.Common;
using Article.Core.IReposiories;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Article.Application.Blog.Command.Create
{
    public class CreateCommandHandler : IRequestHandler<CreateCommand, CreateCommandResponse>
    {
        private readonly ILogger<CreateCommandHandler> _logger;
        private readonly IBlogRepository _blogRepository;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public CreateCommandHandler(ILogger<CreateCommandHandler> logger, IBlogRepository blogRepository, IMapper mapper, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _blogRepository = blogRepository;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }
        public async Task<CreateCommandResponse> Handle(CreateCommand request, CancellationToken cancellationToken)
        {
            var response = new CreateCommandResponse();
            var validator = new CreateCommandValidator();

            var validation = await validator.ValidateAsync(request, cancellationToken);

            if (validation.Errors.Count() > 0)
            {
                response.IsError = true;
                _logger.LogError($"Validation Error.");
                foreach (var error in validation.Errors.Select(x => x.ErrorMessage))
                {
                    response.ErrorsList.Add(error);
                }
                return response;
            }
            else
            {
                var data = _mapper.Map<Article.Core.Entities.Blog>(request);
                await _blogRepository.Add(data);
                await _unitOfWork.CommitAsync();
                if (data.Id == 0)
                {
                    response.IsError = true;
                    _logger.LogError($"Unable to create Blog.");
                    return response;
                }
                else
                {
                    response.IsError = false;
                    response.Id = data.Id;
                    return response;
                }
            }
        }
    }
}
