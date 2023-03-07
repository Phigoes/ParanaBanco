namespace ParanaBanco.Domain.Common
{
    public interface IAudited
    {
        public DateTime CreatedAt { get; }
        public DateTime? LastModifiedAt { get; }
    }
}
