using Plisky.Diagnostics;
using TnLSite.Data;
using TnLSite.Models;

namespace TnLSite.Services;

public sealed class AccountService : IAccountService {
    protected Bilge b;
    private readonly RepositoryBase repository;
    private readonly ITokenService tokenService;

    public AccountService(DynamicTrace dt, RepositoryBase repository, ITokenService tokenService) {
        this.repository = repository;
        this.tokenService = tokenService;
        b = dt.CreateBilge("tnl-service-account");
        b.Info.Flow();
    }

    public string? Login(string userId, string password) {
        b.Info.Flow($"uid {userId} pwd {password}");  // Note do not log passwords in real.

        if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(password)) {
            return null;
        }

        var user = repository.GetUser(userId);
        if (user is null || !user.Enabled || !string.Equals(user.Password, password, StringComparison.Ordinal)) {
            return null;
        }

        user.LastLogin = DateTime.UtcNow;
        repository.SaveUser(user);
        return tokenService.CreateToken(userId);
    }

    public UserDetails? GetUserDetails(string userId, string token) {
        b.Info.Flow($"uid {userId} token {token}"); // or tokens

        if (!tokenService.ValidateToken(userId, token)) {
            return null;
        }

        var user = repository.GetUser(userId);
        return user is null ? null : MapDetails(user);
    }

    public decimal? GetBalance(string userId, string token) {
        b.Info.Flow($"uid {userId} token {token}"); // or tokens
        if (!tokenService.ValidateToken(userId, token)) {
            return null;
        }

        var user = repository.GetUser(userId);
        return user?.Balance;
    }

    public UserDetails? CreateUser(string userId, string userName, string password) {
        b.Info.Flow($"uid {userId} nme {userName}");
        if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(userName) || string.IsNullOrWhiteSpace(password)) {
            return null;
        }

        if (repository.UserExists(userId)) {
            return null;
        }

        var user = new UserRecord {
            UserId = userId,
            UserName = userName,
            Password = password,
            Balance = 0m,
            Enabled = true,
            LastLogin = DateTime.UtcNow
        };

        repository.SaveUser(user);
        return MapDetails(user);
    }

    public (decimal? balance, string? error) UpdateBalance(string userId, string token, decimal amount, DateTime date) {
        b.Info.Flow($"uid {userId} token {token}"); // or tokens

        if (!tokenService.ValidateToken(userId, token)) {
            return (null, "Invalid token.");
        }

        var user = repository.GetUser(userId);
        if (user is null) {
            return (null, "User not found.");
        }

        if (!user.Enabled) {
            return (null, "User is disabled.");
        }

        var newBalance = user.Balance + amount;
        if (amount < 0m && newBalance < 0m) {
            return (null, "Insufficient funds.");
        }

        user.Balance = newBalance;
        repository.SaveUser(user);
        return (newBalance, null);
    }

    private static UserDetails MapDetails(UserRecord user) {
        return new UserDetails {
            UserId = user.UserId,
            UserName = user.UserName,
            Balance = user.Balance,
            Enabled = user.Enabled,
            LastLogin = user.LastLogin
        };
    }
}
