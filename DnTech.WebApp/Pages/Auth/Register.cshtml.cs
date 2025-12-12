using DnTech.WebApp.DTOs;
using DnTech.WebApp.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DnTech.WebApp.Pages.Auth
{
    public class RegisterModel : PageModel
    {
        private readonly AuthService _authService;

        public RegisterModel(AuthService authService)
        {
            _authService = authService;
        }

        [BindProperty]
        public RegisterInputModel Input { get; set; } = new();

        [TempData]
        public string? ErrorMessage { get; set; }

        public void OnGet()
        {
            // Si ya está autenticado, redirigir al dashboard
            if (!string.IsNullOrEmpty(Request.Cookies["AuthToken"]))
            {
                Response.Redirect("/");
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                ErrorMessage = "Por favor completa todos los campos correctamente.";
                return Page();
            }

            // Validar que las contraseñas coincidan
            if (Input.Password != Input.ConfirmPassword)
            {
                ErrorMessage = "Las contraseñas no coinciden.";
                return Page();
            }

            try
            {
                // Llamar a la API
                var result = await _authService.RegisterAsync(new RegisterRequest
                {
                    Email = Input.Email,
                    Password = Input.Password,
                    ConfirmPassword = Input.ConfirmPassword,
                    FirstName = Input.FirstName,
                    LastName = Input.LastName
                });

                if (result.Success && result.Data != null)
                {
                    // Guardar tokens y datos del usuario en cookies
                    var cookieOptions = new CookieOptions
                    {
                        HttpOnly = true,
                        Secure = true,
                        SameSite = SameSiteMode.Strict,
                        Expires = DateTimeOffset.UtcNow.AddHours(24)
                    };

                    Response.Cookies.Append("AuthToken", result.Data.Token, cookieOptions);
                    Response.Cookies.Append("RefreshToken", result.Data.RefreshToken, cookieOptions);
                    Response.Cookies.Append("UserName", $"{result.Data.FirstName} {result.Data.LastName}", cookieOptions);
                    Response.Cookies.Append("UserEmail", result.Data.Email, cookieOptions);

                    // Redirigir al dashboard
                    return RedirectToPage("/Index");
                }
                else
                {
                    ErrorMessage = result.ErrorMessage;
                    return Page();
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = "Ocurrió un error inesperado. Por favor intenta nuevamente.";
                return Page();
            }
        }
    }

    public class RegisterInputModel
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string ConfirmPassword { get; set; } = string.Empty;
        public bool AcceptTerms { get; set; }
    }
}
