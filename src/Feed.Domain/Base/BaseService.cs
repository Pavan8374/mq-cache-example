namespace Feed.Domain.Base
{
    public class BaseService<TEntity> where TEntity : BaseEntity, new()
    {
        private readonly IBaseRepository<TEntity> _baseRepository;
        public BaseService(IBaseRepository<TEntity> baseRepository) 
        {
            _baseRepository = baseRepository;
        }   

        public async Task<TEntity> AddAsync(TEntity entity)
        {
            return await _baseRepository.AddAsync(entity);
        }

        public async Task<TEntity> UpdateAsync(TEntity entity)
        {
            return await _baseRepository.UpdateAsync(entity);
        }
        public async Task<bool> DeleteAsync(TEntity entity)
        {
            return await _baseRepository.DeleteAsync(entity);
        }

        public async Task<IEnumerable<TEntity>> GetAll()
        {
            return await _baseRepository.GetAll();
        }

        public async Task<TEntity> GetById(Guid id)
        {
            return await _baseRepository.GetById(id);
        }

    }
}
