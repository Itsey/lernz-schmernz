using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;
using Shouldly;
using TnLSite.Controllers;
using TnLSite.Repository;
using TnLSite.Services;

namespace TestNLearn.Test;

public class ClientControllerTests {
    [Fact]
    public void Deposit_UsesPersistenceServiceAndReturnsUpdatedUser() {
        var persistence = new StubPersistenceService();
        var controller = new ClientController(null!, null!, NullLogger<ClientController>.Instance, persistence);

        var result = controller.Deposit("user-42", 250);

        result.ShouldBeOfType<OkObjectResult>();
        var ok = (OkObjectResult)result;
        ok.Value.ShouldBeSameAs(persistence.LastUser);
        persistence.LastUser.UserId.ShouldBe("user-42");
        persistence.LastUser.Balance.ShouldBe(250);
    }

    [Fact]
    public void Deposit_PassesUserIdAndAmountToPersistenceService() {
        var persistence = new StubPersistenceService();
        var controller = new ClientController(null!, null!, NullLogger<ClientController>.Instance, persistence);

        controller.Deposit("user-7", -30);

        persistence.LastUserId.ShouldBe("user-7");
        persistence.LastAmount.ShouldBe(-30);
    }

    private sealed class StubPersistenceService : IPersistenceService {
        public UserRecord LastUser { get; private set; } = new();
        public string? LastUserId { get; private set; }
        public long LastAmount { get; private set; }

        public UserRecord SaveUser(UserRecord user) => user;

        public UserRecord? GetUser(string userId) => null;

        public UserRecord Deposit(string userId, long amount) {
            LastUserId = userId;
            LastAmount = amount;
            LastUser = new UserRecord {
                UserId = userId,
                Balance = amount
            };
            return LastUser;
        }

        public UserRecord Withdraw(string userId, long amount) => throw new NotSupportedException();

        public UserRecord Gamble(string userId, long amount, bool won) => throw new NotSupportedException();

        public UserRecord Gift(string fromUserId, string toUserId, long amount) => throw new NotSupportedException();
    }
}
