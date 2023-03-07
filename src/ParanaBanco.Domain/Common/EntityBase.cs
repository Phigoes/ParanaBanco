namespace ParanaBanco.Domain.Common
{
    public abstract class EntityBase: IAudited, ISoftDeletable
    {
        protected EntityBase() { }

        public int Id { get; private set; }

        public DateTime CreatedAt { get; protected set; }

        public bool IsDeleted { get; protected set; }

        public DateTime? LastModifiedAt { get; protected set; }
    }
}
