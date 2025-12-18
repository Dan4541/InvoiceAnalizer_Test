using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DnTech.WebApp.Pages.Files
{
    public class UploadModel : PageModel
    {
        [BindProperty]
        public IFormFile? File { get; set; }

        [BindProperty]
        public string Parameter1 { get; set; } = string.Empty;

        [BindProperty]
        public string Parameter2 { get; set; } = string.Empty;

        [TempData]
        public string? ErrorMessage { get; set; }

        [TempData]
        public string? SuccessMessage { get; set; }

        public void OnGet()
        {
            // Verificar autenticación
            var token = Request.Cookies["AuthToken"];
            if (string.IsNullOrEmpty(token))
            {
                Response.Redirect("/Auth/Login");
                return;
            }

            // TODO: Verificar si tiene rol Admin
            // Por ahora permitimos el acceso
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (File == null)
            {
                ErrorMessage = "Por favor selecciona un archivo.";
                return Page();
            }

            // Validar extensiones permitidas
            var allowedExtensions = new[] { ".pdf", ".jpg", ".jpeg", ".png" };
            var fileExtension = Path.GetExtension(File.FileName).ToLower();

            if (!allowedExtensions.Contains(fileExtension))
            {
                ErrorMessage = "Solo se permiten archivos PDF, JPG o PNG.";
                return Page();
            }

            if (File.Length > 10 * 1024 * 1024) // 10MB
            {
                ErrorMessage = "El archivo no debe superar los 10MB.";
                return Page();
            }

            if (string.IsNullOrWhiteSpace(Parameter1) || string.IsNullOrWhiteSpace(Parameter2))
            {
                ErrorMessage = "Ambos parámetros son requeridos.";
                return Page();
            }

            try
            {
                // TODO: Aquí conectaremos con la API cuando implementemos el endpoint
                // Por ahora solo guardamos temporalmente

                var uploadsPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
                Directory.CreateDirectory(uploadsPath);

                var fileName = $"{Guid.NewGuid()}{fileExtension}";
                var filePath = Path.Combine(uploadsPath, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await File.CopyToAsync(stream);
                }

                SuccessMessage = $"Documento '{File.FileName}' subido exitosamente. Parámetros: {Parameter1}, {Parameter2}";
                return RedirectToPage("/Files/Upload");
            }
            catch (Exception ex)
            {
                ErrorMessage = "Ocurrió un error al subir el documento. Por favor intenta nuevamente.";
                return Page();
            }
        }

    }
}
