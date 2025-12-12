namespace DnTech.Application.UseCases.Auth
{
    public class LoginUserUseCase
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;

        public LoginUserUseCase(
            IUserRepository userRepository,
            IPasswordHasher passwordHasher,
            IJwtTokenGenerator jwtTokenGenerator)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _jwtTokenGenerator = jwtTokenGenerator;
        }

        public async Task<OperationResult<AuthResponse>> ExecuteAsync(
        LoginRequest request,
        CancellationToken cancellationToken = default)
        {
            try
            {
                // Buscar usuario por email
                var user = await _userRepository.GetByEmailAsync(request.Email, cancellationToken);

                if (user == null)
                {
                    return OperationResult<AuthResponse>.Failure("Credenciales inválidas");
                }

                // Verificar contraseña
                if (!_passwordHasher.VerifyPassword(request.Password, user.PasswordHash))
                {
                    return OperationResult<AuthResponse>.Failure("Credenciales inválidas");
                }

                // Verificar si está activo
                if (!user.IsActive)
                {
                    return OperationResult<AuthResponse>.Failure("Usuario inactivo");
                }

                // Actualizar último login
                user.UpdateLastLogin();
                await _userRepository.UpdateAsync(user, cancellationToken);

                // Generar token
                var token = _jwtTokenGenerator.GenerateToken(user);

                var response = new AuthResponse(
                    user.Id,
                    user.Email,
                    user.FirstName,
                    user.LastName,
                    token,
                    DateTime.UtcNow.AddHours(24),
                    user.Roles
                );

                return OperationResult<AuthResponse>.Success(response);
            }
            catch (Exception ex)
            {
                return OperationResult<AuthResponse>.Failure($"Error al iniciar sesión: {ex.Message}");
            }
        }
    }
}
