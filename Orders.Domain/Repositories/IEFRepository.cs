using Orders.Domain.Entities;
using System.Linq.Expressions;

namespace Orders.Domain.Repositories
{
    public interface IEFRepository<TEntity> where TEntity : EFEntity
    {
        IUnitOfWork UnitOfWork { get; }

        IEnumerable<TEntity> GetAll();

        Task<IEnumerable<TEntity>> GetAllAsync();

        Task<IEnumerable<TEntity>> GetAsync(
             Expression<Func<TEntity, bool>> filter = null,
             Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
             string includeProperties = "");

        Task<TEntity> FindAsync(params object[] pks);

        TEntity Add(TEntity entity);

        Task InsertAsync(TEntity entity);

        void Insert(TEntity entity);
 
        Task<TEntity> GetByAsync(Expression<Func<TEntity, bool>> filter);

        Task<TEntity> UpdateAsync(TEntity updateEntity, bool saveChanges = true);

        Task<int> RemoveAsync(TEntity removeEntity);

        int SaveChanges();
    }
}
