using DotNetEnv;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

var builder = WebApplication.CreateBuilder(args);

Env.Load();

string? GetEnv(string key) => Environment.GetEnvironmentVariable(key);
int ParseInt(string? value, int fallback) => int.TryParse(value, out var v) ? v : fallback;

var port = ParseInt(GetEnv("PORT"), 3000);
var sessionDurationSeconds = ParseInt(GetEnv("SESSION_DURATION"), 3600);

var zitadelDomain = GetEnv("ZITADEL_DOMAIN") ?? "https://zitadel.example.com";
var zitadelClientId = GetEnv("ZITADEL_CLIENT_ID") ?? "client-id";
var zitadelClientSecret = GetEnv("ZITADEL_CLIENT_SECRET") ?? "client-secret";
var postLogoutRedirect = GetEnv("ZITADEL_POST_LOGOUT_URL") ?? "http://localhost:3000/auth/logout/callback";

builder.WebHost.ConfigureKestrel(options =>
{
  options.ListenAnyIP(port);
});

builder.Services.AddControllersWithViews();
builder.Services.AddHttpContextAccessor();
builder.Services.AddAntiforgery(options =>
{
  options.HeaderName = "X-CSRF-TOKEN";
});

builder.Services.AddSession(options =>
{
  options.IdleTimeout = TimeSpan.FromSeconds(sessionDurationSeconds);
  options.Cookie.HttpOnly = true;
  options.Cookie.IsEssential = true;
});

builder.Services.AddAuthentication(options =>
{
  options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
  options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
})
.AddCookie(options =>
{
  options.ExpireTimeSpan = TimeSpan.FromSeconds(sessionDurationSeconds);
  options.SlidingExpiration = true;
})
.AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, options =>
{
  options.Authority = zitadelDomain;
  options.ClientId = zitadelClientId;
  options.ClientSecret = zitadelClientSecret;
  options.CallbackPath = "/auth/callback";
  options.SignedOutRedirectUri = postLogoutRedirect;
  options.ResponseType = OpenIdConnectResponseType.Code;
  options.UsePkce = true;
  options.SaveTokens = true;
  options.GetClaimsFromUserInfoEndpoint = true;
  options.TokenValidationParameters.NameClaimType = "name";

  options.Scope.Clear();
  options.Scope.Add("openid");
  options.Scope.Add("profile");
  options.Scope.Add("email");
  options.Scope.Add("offline_access");
  options.Scope.Add("urn:zitadel:iam:user:metadata");
  options.Scope.Add("urn:zitadel:iam:user:resourceowner");
  options.Scope.Add("urn:zitadel:iam:org:projects:roles");

  options.Events.OnRemoteFailure = context =>
  {
    context.Response.Redirect("/auth/error?error=oauthcallback");
    context.HandleResponse();
    return Task.CompletedTask;
  };

  options.Events.OnAccessDenied = context =>
  {
    context.Response.Redirect("/auth/error?error=accessdenied");
    context.HandleResponse();
    return Task.CompletedTask;
  };
  options.Events.OnRedirectToIdentityProviderForSignOut = context =>
  {
    // Force the post-logout redirect to the configured URL instead of the default /signout-callback-oidc
    context.ProtocolMessage.PostLogoutRedirectUri = postLogoutRedirect;
    if (context.Properties != null)
    {
      context.Properties.RedirectUri = postLogoutRedirect;
    }
    return Task.CompletedTask;
  };
});

builder.Services.AddAuthorization();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
  app.UseExceptionHandler("/auth/error?error=auth-error");
}

app.UseStatusCodePagesWithReExecute("/not-found");
app.UseStaticFiles();

app.UseRouting();
app.UseSession();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
