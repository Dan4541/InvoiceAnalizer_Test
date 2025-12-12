using DnTech.Domain.Repositories;

namespace DnTech.Application.UseCases.Auth
{
    public class RefreshTokenUseCase
    {
        private readonly IUserRepository _userRepository;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;

        public RefreshTokenUseCase(
            IUserRepository userRepository,
            IJwtTokenGenerator jwtTokenGenerator)
        {
            _userRepository = userRepository;
            _jwtTokenGenerator = jwtTokenGenerator;
        }

        public async Task<OperationResult<AuthResponse>> ExecuteAsync(
            RefreshTokenRequest request,
            CancellationToken cancellationToken = default)
        {
            try
            {
                // Validar el access token (puede estar expirado)
                var userId = _jwtTokenGenerator.ValidateToken(request.AccessToken);

                if (userId == null)
                {
                    return OperationResult<AuthResponse>.Failure("Token de acceso inválido");
                }

                // Buscar usuario
                var user = await _userRepository.GetByIdAsync(userId.Value, cancellationToken);

                if (user == null)
                {
                    return OperationResult<AuthResponse>.Failure("Usuario no encontrado");
                }

                // Validar refresh token
                if (!user.IsRefreshTokenValid(request.RefreshToken))
                {
                    return OperationResult<AuthResponse>.Failure("Refresh token inválido o expirado");
                }

                // Generar nuevos tokens
                var newAccessToken = _jwtTokenGenerator.GenerateToken(user);
                var newRefreshToken = _jwtTokenGenerator.GenerateRefreshToken();

                // Actualizar refresh token en el usuario
                user.SetRefreshToken(newRefreshToken, DateTime.UtcNow.AddDays(7));
                await _userRepository.UpdateAsync(user, cancellationToken);

                var response = new AuthResponse(
                    user.Id,
                    user.Email,
                    user.FirstName,
                    user.LastName,
                    newAccessToken,
                    newRefreshToken,
                    DateTime.UtcNow.AddHours(24),
                    user.Roles
                );

                return OperationResult<AuthResponse>.Success(response);
            }
            catch (Exception ex)
            {
                return OperationResult<AuthResponse>.Failure($"Error al refrescar token: {ex.Message}");
            }
        }
    }
}
