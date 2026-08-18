using TnLSite.Repository;

namespace TnLSite.Services;

public class PersistenceService : IPersistenceService {
    private readonly Dictionary<string, UserRecord> users = [];

    public UserRecord SaveUser(UserRecord user) {
        users[user.UserId] = user;
        return user;
    }

    public UserRecord? GetUser(string userId) {
        users.TryGetValue(userId, out var user);
        return user;
    }

    public UserRecord Deposit(string userId, long amount) {
        var user = LoadOrCreate(userId);
        user.SetBalanceChangeAtDate(DateTime.UtcNow, amount);
        return SaveUser(user);
    }

    public UserRecord Withdraw(string userId, long amount) {
        var user = LoadOrCreate(userId);

        if (!user.HasSufficientFunds(amount)) {
            throw new InvalidOperationException("Insufficient funds.");
        }

        user.SetBalanceChangeAtDate(DateTime.UtcNow, -amount);
        return SaveUser(user);
    }

    public UserRecord Gamble(string userId, long amount, bool won) {
        var user = LoadOrCreate(userId);

        if (!won && !user.HasSufficientFunds(amount)) {
            throw new InvalidOperationException("Insufficient funds for gamble.");
        }

        long delta = won ? amount : -amount;
        user.SetBalanceChangeAtDate(DateTime.UtcNow, delta);

        return SaveUser(user);
    }

    public UserRecord Gift(string fromUserId, string toUserId, long amount) {
        var fromUser = LoadOrCreate(fromUserId);
        var toUser = LoadOrCreate(toUserId);

        if (!fromUser.HasSufficientFunds(amount)) {
            throw new InvalidOperationException("Insufficient funds to gift.");
        }

        fromUser.SetBalanceChangeAtDate(DateTime.UtcNow, -amount);
        toUser.SetBalanceChangeAtDate(DateTime.UtcNow, amount);

        SaveUser(fromUser);
        SaveUser(toUser);

        return toUser;
    }

    private UserRecord LoadOrCreate(string userId) {
        if (!users.TryGetValue(userId, out var user)) {
            user = new UserRecord {
                UserId = userId,
                UserName = "",
                Password = "",
                Balance = 0,
                ReservedBalance = 0,
                Enabled = true,
                LastLogin = DateTime.UtcNow
            };

            users[userId] = user;
        }

        return user;
    }
}

