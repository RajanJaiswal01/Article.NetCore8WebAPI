namespace Article.Core.Common
{
    public interface IUnitOfWork : IDisposable
    {
        //IGenericRepositories<TEntity> Repository<TEntity>() where TEntity : class;
        Task<long> CommitAsync();
        void Commit();
    }
}
