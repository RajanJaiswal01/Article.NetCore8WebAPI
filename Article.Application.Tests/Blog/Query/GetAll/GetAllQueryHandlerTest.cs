using Article.Application.Blog.Query.GetAll;
using Article.Application.DTO;
using Article.Core.IReposiories;
using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using System.Linq.Expressions;

namespace Article.Application.Tests.Blog.Query.GetAll
{
    public class GetAllQueryHandlerTest
    {
        private readonly Mock<ILogger<GetAllQueryHandler>> _logger;
        private readonly Mock<IBlogRepository> _blogRepository;
        private readonly Mock<IMapper> _mapper;
        public GetAllQueryHandlerTest()
        {
            _logger = new Mock<ILogger<GetAllQueryHandler>>();
            _blogRepository = new Mock<IBlogRepository>();
            _mapper = new Mock<IMapper>();

        }

        [Fact]
        public void Handle_WhenProvidedValidData_shouldReturnListOfData()
        {
            //Arrange
            List<Article.Core.Entities.Blog> blogList = [
                new Article.Core.Entities.Blog { Id = 1, Name = "Learn Python", Description = "Python" },
                new Article.Core.Entities.Blog { Id = 2, Name = "Learn C#", Description = "C#" }
            ];

            List<BlogDTO> blogListDTO = [
                new BlogDTO { Id = 1, Name = "Learn Python", Description = "Python" },
                new BlogDTO { Id = 2, Name = "Learn C#", Description = "C#" }
            ];

            _blogRepository.Setup(repo => repo.GetAll(It.IsAny<Expression<Func<Article.Core.Entities.Blog, bool>>>()))
                              .ReturnsAsync(blogList);
            _mapper.Setup(x => x.Map<List<BlogDTO>>(It.IsAny<Article.Core.Entities.Blog>()> , It.IsAny<Expression<Func<Article.Core.Entities.Blog, object>>[]>)).Returns(blogListDTO.ToList());

            var request = new GetAllQuery();

            var handler = new GetAllQueryHandler(_logger.Object, _blogRepository.Object, _mapper.Object);

            //Act
            var result = handler.Handle(request, It.IsAny<CancellationToken>());


            //Assert
            result.Should().Be(blogListDTO);
            result.Should().NotBeNull();
            result.Should().BeOfType<List<BlogDTO>>();



        }

    }
}
