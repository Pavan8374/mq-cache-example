namespace Feed.Domain.Base
{
    public interface IBaseService<TEntity> where TEntity : BaseEntity, new()
    {
        public Task<TEntity> AddAsync(TEntity entity);
        public Task<TEntity> UpdateAsync(TEntity entity);
        public Task<bool> DeleteAsync(TEntity entity);
        public Task<IEnumerable<TEntity>> GetAll();
        public Task<TEntity> GetById(Guid id);
    }
}
