namespace Yami33.Utility
{
    public interface IAuthService
    {
        Task<string?> LoginAsync(string username, string password);
    }
}
