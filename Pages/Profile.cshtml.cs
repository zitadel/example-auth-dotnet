using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ExampleZitadelAuth.Pages
{
    public class ProfileModel : PageModel
    {
        public string UserJson { get; set; } = "{}";

        public IActionResult OnGet()
        {
            if (!(User?.Identity?.IsAuthenticated ?? false))
            {
                return Redirect("/auth/signin");
            }

            var claims = User?.Claims?.ToDictionary(c => c.Type, c => c.Value) ?? new Dictionary<string, string>();
            UserJson = JsonSerializer.Serialize(claims, new JsonSerializerOptions { WriteIndented = true });

            return Page();
        }
    }
}
