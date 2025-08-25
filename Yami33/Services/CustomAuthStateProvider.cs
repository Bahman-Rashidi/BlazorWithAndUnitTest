using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;
using System.IdentityModel.Tokens.Jwt;


namespace Yami33.Services
{
    //public class CustomAuthStateProvider : AuthenticationStateProvider
    //{
    //    private string? _token;

    //    public void SetToken(string token)
    //    {
    //        _token = token;
    //        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    //    }

    //    public override Task<AuthenticationState> GetAuthenticationStateAsync()
    //    {
    //        var identity = new ClaimsIdentity();

    //        if (!string.IsNullOrEmpty(_token))
    //        {
    //            var handler = new JwtSecurityTokenHandler();
    //            var jwt = handler.ReadJwtToken(_token);

    //            identity = new ClaimsIdentity(jwt.Claims, "jwtAuth");
    //        }

    //        var user = new ClaimsPrincipal(identity);
    //        return Task.FromResult(new AuthenticationState(user));
    //    }
    //}




    public class CustomAuthStateProvider : AuthenticationStateProvider
    {
        private readonly IJSRuntime _jsRuntime;
        private ClaimsPrincipal _anonymous = new ClaimsPrincipal(new ClaimsIdentity());
        private string? _token;

        public CustomAuthStateProvider(IJSRuntime jsRuntime)
        {
            _jsRuntime = jsRuntime;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {

            Console.WriteLine("GetAuthenticationStateAsync called.");
        //    Console.WriteLine("Token: " + (_token ?? "NULL"));
            // اگر توکن قبلاً بارگذاری نشده باشد، سعی کن از LocalStorage بخوانی
            if (string.IsNullOrEmpty(_token))
            {
                try
                {
                    // فقط وقتی Blazor متصل شده می‌توانیم JS را صدا بزنیم
                    _token = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", "authToken");
                }
                catch
                {
                    // اگر prerendering است، خطا نده و ناشناس برگردان
                    return new AuthenticationState(_anonymous);
                }
            }

            if (string.IsNullOrEmpty(_token))
                return new AuthenticationState(_anonymous);

            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(_token);
            var identity = new ClaimsIdentity(jwt.Claims, "jwtAuth");

            return new AuthenticationState(new ClaimsPrincipal(identity));
        }

        public async Task SetTokenAsync(string token)
        {
            _token = token;
            await _jsRuntime.InvokeVoidAsync("localStorage.setItem", "authToken", token);
            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        }

        public async Task LogoutAsync()
        {
            _token = null;
            await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", "authToken");
            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        }
    }
}
