using ExampleZitadelAuth.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ExampleZitadelAuth.Pages.Auth;

public class ErrorModel : PageModel
{
    public string Heading { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;

    public void OnGet([FromQuery] string? error)
    {
        var msg = GetErrorMessage(error);

        Heading = msg.Heading;
        Message = msg.Message;
    }

    private MessageModel GetErrorMessage(string? errorCode)
    {
        var code = string.IsNullOrWhiteSpace(errorCode) ? "default" : errorCode;

        if (code == "accessdenied")
        {
            return new MessageModel
            {
                Heading = "Access Denied",
                Message = "You do not have permission to sign in."
            };
        }

        return new MessageModel
        {
            Heading = "Authentication Error",
            Message = "An unexpected error occurred. Please try again."
        };
    }
}
