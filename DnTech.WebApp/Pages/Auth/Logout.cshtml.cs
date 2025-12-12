using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DnTech.WebApp.Pages.Auth
{
    public class LogoutModel : PageModel
    {
        public IActionResult OnGet()
        {
            // Eliminar todas las cookies de autenticación
            Response.Cookies.Delete("AuthToken");
            Response.Cookies.Delete("RefreshToken");
            Response.Cookies.Delete("UserName");
            Response.Cookies.Delete("UserEmail");

            // Redirigir al login con mensaje de éxito
            TempData["SuccessMessage"] = "Has cerrado sesión exitosamente.";
            return RedirectToPage("/Auth/Login");
        }

        public IActionResult OnPost()
        {
            // Eliminar todas las cookies de autenticación
            Response.Cookies.Delete("AuthToken");
            Response.Cookies.Delete("RefreshToken");
            Response.Cookies.Delete("UserName");
            Response.Cookies.Delete("UserEmail");

            // Redirigir al login con mensaje de éxito
            TempData["SuccessMessage"] = "Has cerrado sesión exitosamente.";
            return RedirectToPage("/Auth/Login");
        }

    }
}
