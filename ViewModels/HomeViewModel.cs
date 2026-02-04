namespace example_dotnet_auth.ViewModels;

public class HomeViewModel
{
    public bool IsAuthenticated { get; set; }
    public string LoginUrl { get; set; } = "/";
}
