using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Mvc;

namespace ExampleZitadelAuth.Controllers;

[Route("auth")]
public class AuthController : ControllerBase
{
    private readonly string _postLoginRedirect;
    private readonly string _postLogoutRedirect;

    public AuthController(IConfiguration configuration)
    {
        _postLoginRedirect = configuration["ZITADEL_POST_LOGIN_URL"] ?? "/profile";
        _postLogoutRedirect = configuration["ZITADEL_POST_LOGOUT_URL"] ?? "http://localhost:3000/auth/logout/callback";
    }

    [HttpPost("signin/provider/zitadel")]
    public IActionResult SignInProvider([FromForm] string? callbackUrl)
    {
        var redirect = string.IsNullOrWhiteSpace(callbackUrl) ? _postLoginRedirect : callbackUrl!;

        return Challenge(new AuthenticationProperties
        {
            RedirectUri = redirect
        }, OpenIdConnectDefaults.AuthenticationScheme);
    }

    [HttpPost("logout")]
    public IActionResult Logout()
    {
        return SignOut(new AuthenticationProperties
        {
            RedirectUri = _postLogoutRedirect
        }, CookieAuthenticationDefaults.AuthenticationScheme, OpenIdConnectDefaults.AuthenticationScheme);
    }
}
