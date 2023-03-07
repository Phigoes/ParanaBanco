using ParanaBanco.Domain.Common;

namespace ParanaBanco.Domain.Interfaces.Repositories
{
    public interface IRepositoryBase<T> where T : EntityBase, IAggregateRoot
    {
        Task<int> Add(T entity);
        Task<IEnumerable<T>> GetAll(bool includeDeleted);
        Task<T?> GetById(int id, bool includeDeleted);
        Task Update(T entity);
        Task Delete(T entity);
    }
}
