using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Mvc;
using example_dotnet_auth.ViewModels;

namespace example_dotnet_auth.Controllers;

[Route("auth")]
public class AuthController : Controller
{
    private readonly IAntiforgery _antiforgery;
    private readonly string _postLogoutRedirect;

    public AuthController(IAntiforgery antiforgery, IConfiguration configuration)
    {
        _antiforgery = antiforgery;
        _postLogoutRedirect = configuration["ZITADEL_POST_LOGOUT_URL"] ?? "http://localhost:3000/auth/logout/callback";
    }

    [HttpGet("signin")]
    public IActionResult SignIn([FromQuery] string? error, [FromQuery] string? callbackUrl)
    {
        var provider = new ProviderViewModel
        {
            Id = "zitadel",
            Name = "ZITADEL",
            SigninUrl = "/auth/signin/provider/zitadel"
        };

        var model = new SignInViewModel
        {
            Providers = new List<ProviderViewModel> { provider },
            CallbackUrl = string.IsNullOrWhiteSpace(callbackUrl) ? "/profile" : callbackUrl
        };

        if (!string.IsNullOrWhiteSpace(error))
        {
            model.Message = GetErrorMessage(error, "signin-error");
        }

        return View("Signin", model);
    }

    [HttpPost("signin/provider/zitadel")]
    public IActionResult SignInProvider([FromForm] string? callbackUrl)
    {
        var redirect = string.IsNullOrWhiteSpace(callbackUrl) ? "/profile" : callbackUrl!;

        return Challenge(new AuthenticationProperties
        {
            RedirectUri = redirect
        }, OpenIdConnectDefaults.AuthenticationScheme);
    }

    [HttpGet("error")]
    public IActionResult ErrorPage([FromQuery] string? error)
    {
        var msg = GetErrorMessage(error, "auth-error");
        var model = new ErrorViewModel
        {
            Heading = msg.Heading,
            Message = msg.Message
        };

        return View("Error", model);
    }

    [HttpGet("logout/callback")]
    public IActionResult LogoutSuccess()
    {
        return View("~/Views/Auth/Logout/Success.cshtml");
    }

    [HttpGet("logout/error")]
    public IActionResult LogoutError([FromQuery] string? reason)
    {
        var model = new ErrorViewModel
        {
            Heading = "Logout unsuccessful",
            Message = string.IsNullOrWhiteSpace(reason) ? "Unable to logout at this time." : reason
        };
        return View("~/Views/Auth/Logout/Error.cshtml", model);
    }

    [HttpPost("logout")]
    public IActionResult Logout()
    {
        return SignOut(new AuthenticationProperties
        {
            RedirectUri = _postLogoutRedirect
        }, CookieAuthenticationDefaults.AuthenticationScheme, OpenIdConnectDefaults.AuthenticationScheme);
    }

    [HttpGet("csrf")]
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Csrf()
    {
        var tokens = _antiforgery.GetAndStoreTokens(HttpContext);
        return Json(new { csrfToken = tokens.RequestToken });
    }

    private MessageViewModel GetErrorMessage(string? errorCode, string category)
    {
        var code = string.IsNullOrWhiteSpace(errorCode) ? "default" : errorCode;

        if (category == "signin-error")
        {
            if (code == "oauthaccountnotlinked")
            {
                return new MessageViewModel
                {
                    Heading = "Account Not Linked",
                    Message = "To confirm your identity, sign in with the same account you used originally."
                };
            }

            return new MessageViewModel
            {
                Heading = "Sign-in Failed",
                Message = "Try signing in with a different account."
            };
        }

        if (code == "accessdenied")
        {
            return new MessageViewModel
            {
                Heading = "Access Denied",
                Message = "You do not have permission to sign in."
            };
        }

        return new MessageViewModel
        {
            Heading = "Authentication Error",
            Message = "An unexpected error occurred. Please try again."
        };
    }
}
