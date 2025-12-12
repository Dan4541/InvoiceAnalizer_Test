using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DnTech.WebApp.Pages
{
    public class IndexModel : PageModel
    {        
        public string UserName { get; set; } = "Usuario";
        public string UserEmail { get; set; } = "usuario@example.com";
        
        public void OnGet()
        {
            // Por ahora valores por defecto
            // Luego obtendremos del token JWT o sesión

            // Verificar si hay información de usuario guardada
            if (Request.Cookies.TryGetValue("UserName", out var userName))
            {
                UserName = userName;
            }

            if (Request.Cookies.TryGetValue("UserEmail", out var userEmail))
            {
                UserEmail = userEmail;
            }

            // Si no está autenticado, redirigir al login
            var token = Request.Cookies["AuthToken"];
            if (string.IsNullOrEmpty(token))
            {
                Response.Redirect("/Auth/Login");
            }
        }
    }
}
