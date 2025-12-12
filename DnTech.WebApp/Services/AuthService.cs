using DnTech.WebApp.DTOs;
using System.Text;
using System.Text.Json;

namespace DnTech.WebApp.Services
{
    public class AuthService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<AuthService> _logger;

        public AuthService(IHttpClientFactory httpClientFactory, ILogger<AuthService> logger)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        public async Task<AuthResult> RegisterAsync(RegisterRequest request)
        {
            try
            {
                var client = _httpClientFactory.CreateClient("AuthAPI");

                var json = JsonSerializer.Serialize(request);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await client.PostAsync("api/Auth/register", content);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var authResponse = JsonSerializer.Deserialize<AuthResponse>(responseContent, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    return new AuthResult
                    {
                        Success = true,
                        Data = authResponse
                    };
                }
                else
                {
                    var errorResponse = JsonSerializer.Deserialize<ErrorResponse>(responseContent, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    return new AuthResult
                    {
                        Success = false,
                        ErrorMessage = errorResponse?.Message ?? "Error al registrar usuario"
                    };
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al llamar a la API de registro");
                return new AuthResult
                {
                    Success = false,
                    ErrorMessage = "Error de conexión con el servidor. Por favor intenta más tarde."
                };
            }
        }

        public async Task<AuthResult> LoginAsync(LoginRequest request)
        {
            try
            {
                var client = _httpClientFactory.CreateClient("AuthAPI");

                var json = JsonSerializer.Serialize(request);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await client.PostAsync("api/Auth/login", content);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var authResponse = JsonSerializer.Deserialize<AuthResponse>(responseContent, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    return new AuthResult
                    {
                        Success = true,
                        Data = authResponse
                    };
                }
                else
                {
                    var errorResponse = JsonSerializer.Deserialize<ErrorResponse>(responseContent, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    return new AuthResult
                    {
                        Success = false,
                        ErrorMessage = errorResponse?.Message ?? "Credenciales inválidas"
                    };
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al llamar a la API de login");
                return new AuthResult
                {
                    Success = false,
                    ErrorMessage = "Error de conexión con el servidor. Por favor intenta más tarde."
                };
            }
        }

    }
}
