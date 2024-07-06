using Article.Core.Entities;
using Article.Core.IReposiories;
using Article.Infrastructure.Common;

namespace Article.Infrastructure.Repositories
{
    public class PostsRepository:GenericRepository<Posts>,IPostsRepository
    {
        public PostsRepository(IDbFactory dbFactory):base(dbFactory)
        {
            
        }
    }
}
