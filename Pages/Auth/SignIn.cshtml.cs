using ExampleZitadelAuth.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ExampleZitadelAuth.Pages.Auth;

public class SignInModel : PageModel
{
    private readonly string _postLoginRedirect;

    public SignInModel(IConfiguration configuration)
    {
        _postLoginRedirect = configuration["ZITADEL_POST_LOGIN_URL"] ?? "/profile";
    }

    public List<ProviderModel> Providers { get; set; } = new();
    public string CallbackUrl { get; set; } = "/profile";
    public MessageModel? Message { get; set; }

    public void OnGet([FromQuery] string? error, [FromQuery] string? callbackUrl)
    {
        var provider = new ProviderModel
        {
            Id = "zitadel",
            Name = "ZITADEL",
            SigninUrl = "/auth/signin/provider/zitadel"
        };

        Providers = new List<ProviderModel> { provider };
        CallbackUrl = string.IsNullOrWhiteSpace(callbackUrl) ? _postLoginRedirect : callbackUrl;

        if (!string.IsNullOrWhiteSpace(error))
        {
            Message = GetErrorMessage(error);
        }
    }

    private MessageModel GetErrorMessage(string? errorCode)
    {
        var code = string.IsNullOrWhiteSpace(errorCode) ? "default" : errorCode;

        if (code == "oauthaccountnotlinked")
        {
            return new MessageModel
            {
                Heading = "Account Not Linked",
                Message = "To confirm your identity, sign in with the same account you used originally."
            };
        }

        return new MessageModel
        {
            Heading = "Sign-in Failed",
            Message = "Try signing in with a different account."
        };
    }
}
