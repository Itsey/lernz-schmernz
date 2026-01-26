using TnLSite.Models;

namespace TnLSite.Services;

public interface IAccountService {
    string? Login(string userId, string password);
    UserDetails? GetUserDetails(string userId, string token);
    decimal? GetBalance(string userId, string token);
    UserDetails? CreateUser(string userId, string userName, string password);
    (decimal? balance, string? error) UpdateBalance(string userId, string token, decimal amount, DateTime date);
}
