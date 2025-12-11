namespace DnTech.Application.DTOs.Auth
{
    public record UserDto(
    Guid Id,
    string Email,
    string FirstName,
    string LastName,
    bool IsActive,
    DateTime CreatedAt,
    DateTime? LastLoginAt,
    List<string> Roles
);
}
