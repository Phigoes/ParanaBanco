using Microsoft.EntityFrameworkCore;
using ParanaBanco.Domain.Common;
using ParanaBanco.Domain.Interfaces.Repositories;

namespace ParanaBanco.Infrastructure.Persistence.Repositories
{
    public class RepositoryBase<T> : IRepositoryBase<T> where T : EntityBase, IAggregateRoot
    {
        protected readonly ParanaBancoDbContext _context;
        private readonly DbSet<T> _entities;

        public RepositoryBase(ParanaBancoDbContext context)
        {
            _context = context;
            _entities = _context.Set<T>();
        }

        public async Task<int> Add(T entity)
        {
            await _entities.AddAsync(entity);
            await _context.SaveChangesAsync();

            return entity.Id;
        }

        public async Task Delete(T entity)
        {
            _entities.Remove(entity);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<T>> GetAll(bool includeDeleted)
        {
            return includeDeleted ? 
                await _entities.AsNoTracking().ToListAsync() : await _entities.AsNoTracking().Where(x => x.IsDeleted == false).ToListAsync();
        }

        public async Task<T?> GetById(int id, bool includeDeleted)
        {
            var entity = await _entities.FindAsync(id);
            if (entity == null || entity.IsDeleted && !includeDeleted)
                return null;

            return entity;
        }

        public async Task Update(T entity)
        {
            _entities.Update(entity);
            await _context.SaveChangesAsync();
        }
    }
}
