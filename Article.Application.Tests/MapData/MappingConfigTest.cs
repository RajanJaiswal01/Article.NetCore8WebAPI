using Article.Application.DTO;
using Article.Application.MapData;
using AutoFixture;
using AutoMapper;
using FluentAssertions;

namespace Article.Application.Tests.MapData
{
    public class MappingConfigTest
    {
        private readonly IMapper _mapper;
        private readonly IFixture _fixture;
        public MappingConfigTest()
        {
            var config = new MapperConfiguration(cfg => {
                cfg.AddProfile<MappingConfig>();
            });

            _mapper = config.CreateMapper();
            _fixture = new Fixture();
            _fixture.Behaviors.Remove(new ThrowingRecursionBehavior());
            _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
        }

        [Fact]
        public void MappingBlog_whenProvidedValidBlog_ShouldMapintoBlogDtoSuccessfully()
        {
            //Arrange
            var blogdata = new Article.Core.Entities.Blog() { Name ="rajan", Description = "jdksfh"};
            //var blogdata = _fixture.Create<Article.Core.Entities.Blog>();

            //Act
            var result = _mapper.Map<BlogDTO>(blogdata);

            //Assert
            result.Should().BeOfType<BlogDTO>();
            result.Should().NotBeNull();
            result.Should().BeAssignableTo<BlogDTO>();
            
        }
    }
}
