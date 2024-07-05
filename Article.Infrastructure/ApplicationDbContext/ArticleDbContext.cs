using Article.Core.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Article.Infrastructure.ApplicationDbContext
{
    public class ArticleDbContext:IdentityDbContext<User>
    {
        public ArticleDbContext(DbContextOptions<ArticleDbContext> dbContext) : base(dbContext)
        {
            
        }


        DbSet<Author> Author { get; set; }
        DbSet<Blog> Blog { get; set; }
        DbSet<Posts> Posts { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<User>()
                .OwnsOne(r => r.Address);

            builder.Entity<Blog>()
                .HasMany(r => r.Posts)
                .WithOne(r => r.Blog)
                .HasForeignKey(r => r.BlogId);

            builder.Entity<Posts>()
                .HasOne(r => r.Author)
                .WithMany(r => r.Posts)
                .HasForeignKey(r => r.AuthorId);
                
            builder.Entity<User>()
                .HasMany(r => r.Authors)
                .WithOne(r => r.User)
                .HasForeignKey(r => r.UserId);   
        }
    }
}
