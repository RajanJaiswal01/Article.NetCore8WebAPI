using Article.Application.Blog.Command.Create;
using Article.Application.DTO;
using Article.Core.Common;
using Article.Core.Entities;
using Article.Core.IReposiories;
using AutoMapper;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Article.Application.Blog.Command.Update
{
    public class UpdateCommandHandler : IRequestHandler<UpdateCommand, UpdateCommandResponse>
    {
        private readonly ILogger<UpdateCommandHandler> _logger;
        private readonly IBlogRepository _blogRepository;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateCommandHandler(ILogger<UpdateCommandHandler> logger, IBlogRepository blogRepository, IMapper mapper, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _blogRepository = blogRepository;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<UpdateCommandResponse> Handle(UpdateCommand request, CancellationToken cancellationToken)
        {
            var response = new UpdateCommandResponse();
            var validator = new UpdateCommandValidation();

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
                await _blogRepository.Update(data);
                await _unitOfWork.CommitAsync();

                var responseData = _mapper.Map<BlogDTO>(request);
                responseData.Posts = _mapper.Map<List<PostDTO>>(request.Posts);
                response.IsError = false;
                response.Id = data.Id;
                response.Data = responseData;
                return response;
            }
        }
    }
}
