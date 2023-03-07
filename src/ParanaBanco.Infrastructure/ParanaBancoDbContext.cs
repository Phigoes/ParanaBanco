using Microsoft.EntityFrameworkCore;
using ParanaBanco.Domain.Entities;
using System.Reflection;

namespace ParanaBanco.Infrastructure
{
    public class ParanaBancoDbContext : DbContext
    {
        public ParanaBancoDbContext() { }

        public ParanaBancoDbContext(DbContextOptions<ParanaBancoDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
    }
}
