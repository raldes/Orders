using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Orders.Domain.Entities;
using Orders.Domain.Repositories;
using Orders.Infra.Database;
using System.Linq.Expressions;

namespace Orders.Infra.Repositories
{
    public class EFRepository<TEntity> : IEFRepository<TEntity> where TEntity : EFEntity
    {
        protected readonly OrdersDbContext _context;
        protected readonly ILogger<TEntity> _logger;

        DbSet<TEntity> _collection;

        public IUnitOfWork UnitOfWork
        {
            get
            {
                return _context;
            }
        }

        public EFRepository(OrdersDbContext _context, ILogger<TEntity> logger)
        {
            this._context = _context;

            _logger = logger;

            _collection = _context.Set<TEntity>();
        }


        public IEnumerable<TEntity> GetAll()
        {
            try
            {
                var ents = _collection.AsNoTracking().AsEnumerable();
                return ents;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public Task<IEnumerable<TEntity>> GetAllAsync()
        {
            return Task.Run(() =>
            {
                return GetAll();
            });
        }

        public async Task<IEnumerable<TEntity>> GetAsync(
             Expression<Func<TEntity, bool>> filter = null,
             Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
             string includeProperties = "")
        {
            IQueryable<TEntity> query = _collection;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            foreach (var includeProperty in includeProperties.Split
                (new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProperty);
            }

            if (orderBy != null)
            {
                return await orderBy(query).ToListAsync();
            }
            else
            {
                return await query.ToListAsync();
            }
        }

        public async Task<TEntity> FindAsync(params object[] pks)
        {
            return await _collection.FindAsync(pks);
        }

        public async Task InsertAsync(TEntity entity)
        {
            await _collection.AddAsync(entity);
        }

        public TEntity Add(TEntity entity)
        {
            return _collection.Add(entity).Entity;
        }

        public virtual void Insert(TEntity entity)
        {
            _collection.Add(entity);
        }

        public async Task<TEntity> GetByAsync(Expression<Func<TEntity, bool>> filter)
        {
            if (filter == null)
            {
                throw new ArgumentNullException(nameof(filter), $"The parameter filter can not be null");
            }

            try
            {
                var query = _collection.Local.AsQueryable();

                var first = query.FirstOrDefault(filter);

                TEntity? result = await Task.FromResult(first);

                if (result == null)
                {
                    result = await _collection.AsNoTracking().FirstOrDefaultAsync(filter);
                }

                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public TEntity Update(TEntity updateEntity, bool saveChanges = true)
        {
            if (updateEntity == null)
            {
                throw new ArgumentNullException(nameof(updateEntity), $"The parameter updateEntity can not be null");
            }

            try
            {
                _collection.Attach(updateEntity);

                _context.Entry(updateEntity).State = EntityState.Modified;

                if (saveChanges)
                {
                    var result = SaveChanges();
                }

                return updateEntity;
            }

            catch (Exception)
            {

                throw;
            }
        }

        public Task<TEntity> UpdateAsync(TEntity updateEntity, bool saveChanges = true)
        {
            return Task.Run(() =>
            {
                return Update(updateEntity, saveChanges);
            });
        }

        public Task<int> RemoveAsync(TEntity removeEntity)
        {
            return Task.Run(() =>
            {
                return Remove(removeEntity);
            });
        }

        public int Remove(TEntity removeEntity)
        {
            if (removeEntity == null)
            {
                throw new ArgumentNullException(nameof(removeEntity), $"The parameter removeEntity can not be null");
            }

            try
            {
                _collection.Attach(removeEntity);

                _context.Entry(removeEntity).State = EntityState.Deleted;

                var result = SaveChanges();

                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public TEntity Find(params object[] pks)
        {
            return _collection.Find(pks);
        }

        public int SaveChanges()
        {
            int results = 0;
            try
            {
                results = _context.SaveChanges();
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError("DbUpdateException", ex);
                throw ex;
            }
            catch (Exception ex)
            {
                _logger.LogError("Generic Exception", ex);
                throw;
            }

            return results;
        }
    }
}
