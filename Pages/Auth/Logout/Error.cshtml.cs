using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ExampleZitadelAuth.Pages.Auth.Logout;

public class ErrorModel : PageModel
{
    public string Heading { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;

    public void OnGet([FromQuery] string? reason)
    {
        Heading = "Logout unsuccessful";
        Message = string.IsNullOrWhiteSpace(reason) ? "Unable to logout at this time." : reason;
    }
}
