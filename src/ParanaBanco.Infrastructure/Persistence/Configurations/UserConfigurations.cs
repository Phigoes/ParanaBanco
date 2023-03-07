using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ParanaBanco.Domain.Entities;

namespace ParanaBanco.Infrastructure.Persistence.Configurations
{
    public class UserConfigurations : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasKey(u => u.Id);

            builder.Property(u => u.FullName).IsRequired().HasMaxLength(100);

            builder.OwnsOne(u => u.Email, e =>
            {
                e.Property(p => p.Address)
                .IsRequired()
                .HasMaxLength(100);

                e.HasIndex(p => p.Address).IsUnique();
            });
        }
    }
}
