using Article.Infrastructure.ApplicationDbContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Article.Infrastructure.Common
{
    public interface IDbFactory
    {
        ArticleDbContext Init();
    }
}
