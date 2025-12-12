namespace DnTech.Application.UseCases.Auth
{
    public class RegisterUserUseCase
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;

        public RegisterUserUseCase(
            IUserRepository userRepository,
            IPasswordHasher passwordHasher,
            IJwtTokenGenerator jwtTokenGenerator)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _jwtTokenGenerator = jwtTokenGenerator;
        }

        public async Task<OperationResult<AuthResponse>> ExecuteAsync(
        RegisterRequest request,
        CancellationToken cancellationToken = default)
        {
            try
            {
                // Verificar si el email ya existe
                if (await _userRepository.ExistsByEmailAsync(request.Email, cancellationToken))
                {
                    return OperationResult<AuthResponse>.Failure("El email ya está registrado");
                }

                // Hash de la contraseña
                var passwordHash = _passwordHasher.HashPassword(request.Password);

                // Crear usuario
                var user = User.Create(
                    request.Email,
                    passwordHash,
                    request.FirstName,
                    request.LastName
                );

                // Guardar en BD
                await _userRepository.AddAsync(user, cancellationToken);

                // Generar token JWT
                var token = _jwtTokenGenerator.GenerateToken(user);

                var response = new AuthResponse(
                    user.Id,
                    user.Email,
                    user.FirstName,
                    user.LastName,
                    token,
                    DateTime.UtcNow.AddHours(24), // Token expira en 24 horas
                    user.Roles
                );

                return OperationResult<AuthResponse>.Success(response);
            }
            catch (DuplicateEmailException ex)
            {
                return OperationResult<AuthResponse>.Failure(ex.Message);
            }
            catch (Exception ex)
            {
                return OperationResult<AuthResponse>.Failure($"Error al registrar usuario: {ex.Message}");
            }
        }



    }
}
