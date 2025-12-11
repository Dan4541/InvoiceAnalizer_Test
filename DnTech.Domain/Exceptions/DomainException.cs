namespace DnTech.Domain.Exceptions
{
    public abstract class DomainException : Exception
    {
        protected DomainException(string message) : base(message) { }

        protected DomainException(string message, Exception innerException)
            : base(message, innerException) { }
    }

    public class UserNotFoundException : DomainException
    {
        public UserNotFoundException(Guid userId)
            : base($"Usuario con ID {userId} no fue encontrado.") { }

        public UserNotFoundException(string email)
            : base($"Usuario con email {email} no fue encontrado.") { }
    }

    public class DuplicateEmailException : DomainException
    {
        public DuplicateEmailException(string email)
            : base($"Ya existe un usuario con el email {email}.") { }
    }

    public class InvalidCredentialsException : DomainException
    {
        public InvalidCredentialsException()
            : base("Las credenciales proporcionadas son inválidas.") { }
    }

}
