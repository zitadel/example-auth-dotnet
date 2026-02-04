namespace example_dotnet_auth.ViewModels;

public class SignInViewModel
{
    public List<ProviderViewModel> Providers { get; set; } = new();
    public string CallbackUrl { get; set; } = "/profile";
    public MessageViewModel? Message { get; set; }
}
