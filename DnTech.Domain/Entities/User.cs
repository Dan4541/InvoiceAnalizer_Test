namespace DnTech.Domain.Entities
{
    public class User
    {
        public Guid Id { get; private set; }
        public string Email { get; private set; }
        public string PasswordHash { get; private set; }
        public string FirstName { get; private set; }
        public string LastName { get; private set; }
        public bool IsActive { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime? LastLoginAt { get; private set; }
        public List<string> Roles { get; private set; }

        // Constructor privado para EF Core
        private User()
        {
            Roles = new List<string>();
        }

        // Factory method para crear un usuario
        public static User Create(
            string email,
            string passwordHash,
            string firstName,
            string lastName)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Email no puede estar vacío", nameof(email));

            if (string.IsNullOrWhiteSpace(passwordHash))
                throw new ArgumentException("Password hash no puede estar vacío", nameof(passwordHash));

            return new User
            {
                Id = Guid.NewGuid(),
                Email = email.ToLowerInvariant(),
                PasswordHash = passwordHash,
                FirstName = firstName,
                LastName = lastName,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                Roles = new List<string> { "User" } // Rol por defecto
            };
        }

        public void UpdateLastLogin()
        {
            LastLoginAt = DateTime.UtcNow;
        }

        public void Deactivate()
        {
            IsActive = false;
        }

        public void AddRole(string role)
        {
            if (!Roles.Contains(role))
                Roles.Add(role);
        }

        public void RemoveRole(string role)
        {
            Roles.Remove(role);
        }
    }
}
