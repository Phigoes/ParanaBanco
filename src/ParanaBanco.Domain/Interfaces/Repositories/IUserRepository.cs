using ParanaBanco.Domain.Entities;

namespace ParanaBanco.Domain.Interfaces.Repositories
{
    public interface IUserRepository : IRepositoryBase<User>
    {
        Task<User?> GetByEmail(string email);

        Task<IEnumerable<User>> GetOnlyDeleted();
    }
}
