using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ExampleZitadelAuth.Pages
{
    public class IndexModel : PageModel
    {
        public bool IsAuthenticated { get; set; }
        public string LoginUrl { get; set; } = "/";

        public void OnGet()
        {
            IsAuthenticated = User?.Identity?.IsAuthenticated ?? false;
            LoginUrl = "/auth/signin/provider/zitadel";
        }
    }
}
