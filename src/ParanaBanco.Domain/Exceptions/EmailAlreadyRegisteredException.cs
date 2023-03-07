namespace ParanaBanco.Domain.Exceptions
{
    public class EmailAlreadyRegisteredException : Exception
    {
        public EmailAlreadyRegisteredException() : base("Email already registered.")
        {
        }
    }
}
