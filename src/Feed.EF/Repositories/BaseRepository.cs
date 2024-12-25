using Feed.Domain.Base;
using Microsoft.EntityFrameworkCore;

namespace Feed.EF.Repositories
{
    public abstract class BaseRepository<TEntity, TDbContext> where TEntity : BaseEntity, new()
    {
        private readonly ApplicationDbContext _context;
        protected BaseRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public virtual async Task<TEntity> AddAsync(TEntity entity)
        {
            entity.Id = Guid.NewGuid();
            entity.IsActive = true;
            entity.CreatedAt = DateTime.Now;
            entity.UpdatedAt = null;
            _context.Set<TEntity>().Add(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public virtual async Task<TEntity> UpdateAsync(TEntity entity)
        {
            entity.UpdatedAt = null;
            if (_context.Entry(entity).State == EntityState.Detached)
            {
                _context.Set<TEntity>().Update(entity);
            }
            await _context.SaveChangesAsync();
            return entity;
        }

        public virtual async Task<bool> DeleteAsync(TEntity entity)
        {
            try
            {
                entity.IsActive = false;
                entity.UpdatedAt = DateTime.UtcNow;
                _context.Set<TEntity>().Update(entity);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex) 
            {
                return false;
            }
            
        }

        public virtual async Task<IEnumerable<TEntity>> GetAll()
        {
            return await GetQuery().ToListAsync();
        }
        public virtual async Task<TEntity> GetById(Guid id)
        {
            return await GetQuery().FirstOrDefaultAsync(x => x.Id == id);
        }

        protected virtual IQueryable<TEntity> GetQuery()
        {
            var query = _context.Set<TEntity>().AsNoTracking().Where(x => x.IsActive == true);
            return query;
        }
    }
}
