using DnTech.Domain.Constants;

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
        public string? RefreshToken { get; private set; }
        public DateTime? RefreshTokenExpiryTime { get; private set; }

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
            string lastName,
            bool isAdmin = false)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Email no puede estar vacío", nameof(email));

            if (string.IsNullOrWhiteSpace(passwordHash))
                throw new ArgumentException("Password hash no puede estar vacío", nameof(passwordHash));

            var roles = new List<string> { UserRoles.User };

            if (isAdmin)
            {
                roles.Add(UserRoles.Admin);
            }

            return new User
            {
                Id = Guid.NewGuid(),
                Email = email.ToLowerInvariant(),
                PasswordHash = passwordHash,
                FirstName = firstName,
                LastName = lastName,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                Roles = roles
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
            if (!UserRoles.IsValidRole(role))
                throw new ArgumentException($"El rol '{role}' no es válido", nameof(role));

            if (!this.Roles.Contains(role))
                this.Roles.Add(role);
        }

        public void RemoveRole(string role)
        {
            // No permitir eliminar el rol User si es el único
            if (role == UserRoles.User && this.Roles.Count == 1)
                throw new InvalidOperationException("No se puede eliminar el rol User si es el único rol del usuario");

            this.Roles.Remove(role);
        }

        public bool HasRole(string role)
        {
            return this.Roles.Contains(role);
        }

        public bool IsAdmin()
        {
            return this.Roles.Contains(UserRoles.Admin);
        }

        public void SetRefreshToken(string refreshToken, DateTime expiryTime)
        {
            RefreshToken = refreshToken;
            RefreshTokenExpiryTime = expiryTime;
        }

        public void ClearRefreshToken()
        {
            RefreshToken = null;
            RefreshTokenExpiryTime = null;
        }

        public bool IsRefreshTokenValid(string refreshToken)
        {
            return RefreshToken == refreshToken &&
                   RefreshTokenExpiryTime.HasValue &&
                   RefreshTokenExpiryTime.Value > DateTime.UtcNow;
        }
    }
}
