using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.Extensions.Configuration;

namespace SimpleCmsBlazor.Models;

public class CustomAuthorizationMessageHandler : AuthorizationMessageHandler
{
    public CustomAuthorizationMessageHandler(IConfiguration config, IAccessTokenProvider provider, NavigationManager navigation) : base(provider, navigation)
    {
        var scope =  config.GetValue<string>("Scope");
        var apiUrl = config.GetValue<string>("ApiUrl");
        ConfigureHandler(authorizedUrls: new[] { apiUrl }, scopes: new[] { scope });
    }
}
