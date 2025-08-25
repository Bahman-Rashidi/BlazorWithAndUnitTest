
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Yami33.Utility;
namespace Yami33.Services
{
    public class AuthService:IAuthService
    {
        private readonly HttpClient _httpClient;

        public AuthService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<string> LoginAsync(string username, string password)
        {
            var response = await _httpClient.PostAsJsonAsync("api/auth/login", new
            {
                Username = username,
                Password = password
            });

            if (!response.IsSuccessStatusCode)
                return null;

            var result = await response.Content.ReadFromJsonAsync<LoginResponse>();
            return result?.Token;
        }
    }
}
