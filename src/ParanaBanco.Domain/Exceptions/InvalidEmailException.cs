namespace ParanaBanco.Domain.Exceptions
{
    public class InvalidEmailException : Exception
    {
        public InvalidEmailException() : base("Formato de e-mail inválido.")
        {

        }
    }
}
