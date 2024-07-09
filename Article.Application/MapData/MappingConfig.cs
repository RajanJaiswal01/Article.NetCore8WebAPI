using Article.Application.Blog.Command.Create;
using Article.Application.DTO;
using AutoMapper;

namespace Article.Application.MapData
{
    public class MappingConfig:Profile
    {
        public MappingConfig()
        {
            CreateMap<Article.Core.Entities.Blog,BlogDTO>().ReverseMap();
            CreateMap<Article.Core.Entities.Post,PostDTO>()
                .ForMember(dest => dest.AuthorName, opt => opt.MapFrom(src => src.Author.User.UserName))
                .ReverseMap();

            CreateMap<Article.Core.Entities.Blog, BlogDTO>()
            .ForMember(dest => dest.Posts, opt => opt.MapFrom(src => src.Posts))
            .ReverseMap();

            CreateMap<CreateCommand, Article.Core.Entities.Blog>()
            .ForMember(dest => dest.Posts, opt => opt.MapFrom(src => src.Posts))
            .ReverseMap();


            //CreateMap<Article.Core.Entities.User, AuthorDTO>()
            //    .ForMember(dest => dest.City, opt => opt.MapFrom(src => src.Address.City))
            //    .ForMember(dest => dest.Street, opt => opt.MapFrom(src => src.Address.Street))
            //    .ForMember(dest => dest.Country, opt => opt.MapFrom(src => src.Address.Country))
            //    .ForMember(dest => dest.PostalCode, opt => opt.MapFrom(src => src.Address.PostalCode)).ReverseMap();
        }
    }
}
