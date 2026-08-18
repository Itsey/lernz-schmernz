using TnLSite.Repository;

namespace TnLSite.Services;

public interface IPersistenceService {
    UserRecord SaveUser(UserRecord user);
    UserRecord? GetUser(string userId);

    // operations
    UserRecord Deposit(string userId, long amount);
    UserRecord Withdraw(string userId, long amount);
    UserRecord Gamble(string userId, long amount, bool won);
    UserRecord Gift(string fromUserId, string toUserId, long amount);
}

