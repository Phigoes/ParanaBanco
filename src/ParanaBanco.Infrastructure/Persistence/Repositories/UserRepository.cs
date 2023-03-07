using Microsoft.EntityFrameworkCore;
using ParanaBanco.Domain.Entities;
using ParanaBanco.Domain.Interfaces.Repositories;
using ParanaBanco.Domain.ValueObjects;

namespace ParanaBanco.Infrastructure.Persistence.Repositories
{
    public class UserRepository : RepositoryBase<User>, IUserRepository
    {
        public UserRepository(ParanaBancoDbContext context) : base(context)
        {
        }

        public async Task<User?> GetByEmail(string email)
        {
            var entities = await _context.Users.AsNoTracking().ToListAsync();
            var entity = entities.SingleOrDefault(x => x.Email.Address == email);

            return entity;
        }

        public async Task<IEnumerable<User>> GetOnlyDeleted()
        {
            return await _context.Users.AsNoTracking().Where(x => x.IsDeleted == true).ToListAsync();
        }
    }
}
