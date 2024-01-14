using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;

namespace SimpleCmsBlazor.Models;

public class CustomAuthorizationMessageHandler : AuthorizationMessageHandler
{
    public CustomAuthorizationMessageHandler(IConfiguration config, IAccessTokenProvider provider, NavigationManager navigation) : base(provider, navigation)
    {
        var scope = config.GetValue<string>("Scope") ?? string.Empty;
        var apiUrl = config.GetValue<string>("ApiUrl") ?? string.Empty;
        ConfigureHandler(authorizedUrls: [apiUrl], scopes: [scope]);
    }
}