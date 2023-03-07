using ParanaBanco.Domain.Common;
using ParanaBanco.Domain.ValueObjects;

namespace ParanaBanco.Domain.Entities
{
    public class User : EntityBase, IAggregateRoot
    {
        public User() { }

        public User(string fullName, Email email)
        {
            FullName = fullName;
            Email = email;
            CreatedAt = DateTime.Now;
        }

        public string FullName { get; private set; }
        public Email Email { get; private set; }

        public void Delete()
        {
            IsDeleted = true;
            Modify();
        }

        public void Restore()
        {
            IsDeleted = false;
            Modify();
        }

        public void Modify()
        {
            LastModifiedAt = DateTime.Now;
        }

        public void ModifyUser(string fullName, Email email)
        {
            FullName = fullName;
            Email = email;
            Modify();
        }
    }
}
