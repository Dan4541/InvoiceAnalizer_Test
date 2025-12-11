namespace DnTech.Application.DTOs.Auth
{
    public record AuthResponse(
        Guid UserId,
        string Email,
        string FirstName,
        string LastName,
        string Token,
        DateTime ExpiresAt,
        List<string> Roles
    );
}
