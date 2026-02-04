using System.Text.Json;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using example_dotnet_auth.ViewModels;

namespace example_dotnet_auth.Controllers;

public class HomeController : Controller
{
  [HttpGet("/")]
  public IActionResult Index()
  {
    var model = new HomeViewModel
    {
      IsAuthenticated = User?.Identity?.IsAuthenticated ?? false,
      LoginUrl = "/auth/signin/provider/zitadel"
    };

    return View("Index", model);
  }

  [HttpGet("/profile")]
  public IActionResult Profile()
  {
    if (!(User?.Identity?.IsAuthenticated ?? false))
    {
      return Redirect("/auth/signin");
    }

    var claims = User?.Claims?.ToDictionary(c => c.Type, c => c.Value) ?? new Dictionary<string, string>();
    var json = JsonSerializer.Serialize(claims, new JsonSerializerOptions { WriteIndented = true });

    var model = new ProfileViewModel
    {
      UserJson = json
    };

    return View("Profile", model);
  }

  [HttpGet("/not-found")]
  public IActionResult NotFoundPage()
  {
    Response.StatusCode = StatusCodes.Status404NotFound;
    return View("NotFound");
  }
}
