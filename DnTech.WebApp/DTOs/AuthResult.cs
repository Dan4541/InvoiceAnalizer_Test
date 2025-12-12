namespace DnTech.WebApp.DTOs
{
    public class AuthResult
    {
        public bool Success { get; set; }
        public AuthResponse? Data { get; set; }
        public string ErrorMessage { get; set; } = string.Empty;
    }
}
