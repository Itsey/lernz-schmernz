using Plisky.Plumbing;
using Shouldly;
using TnLSite.Repository;

namespace TestNLearn.Test;

public class UserRecordTests {
    [Fact]
    public void RecalculateBalanceAndReserve_EmptyTransactions_SetsZeroBalanceAndReserve() {
        var user = new UserRecord();

        user.SetBalanceChangeAtDate(DateTime.UtcNow, 0);

        user.Balance.ShouldBe(0);
        user.ReservedBalance.ShouldBe(0);
        user.Transactions.Count.ShouldBe(1);
    }

    [Fact]
    public void RecalculateBalanceAndReserve_PastTransactions_CalculatesRunningBalanceAndUpdatesRecords() {
        var user = new UserRecord();
        var now = DateTime.UtcNow;

        user.SetBalanceChangeAtDate(now.AddHours(-3), 100);
        user.SetBalanceChangeAtDate(now.AddHours(-2), -30);
        user.SetBalanceChangeAtDate(now.AddHours(-1), 50);

        user.Balance.ShouldBe(120);
        user.ReservedBalance.ShouldBe(0);

        user.Transactions[0].Balance.ShouldBe(100);
        user.Transactions[1].Balance.ShouldBe(70);
        user.Transactions[2].Balance.ShouldBe(120);
    }

    [Fact]
    public void RecalculateBalanceAndReserve_UnorderedTransactions_SortsByDateAndCalculatesCorrectly() {
        var user = new UserRecord();
        var now = DateTime.UtcNow;
        var xx = InternalUtil.GetCallingStackFrame();
        // Added out of order
        user.SetBalanceChangeAtDate(now.AddHours(-1), 50);
        user.SetBalanceChangeAtDate(now.AddHours(-3), 100);
        user.SetBalanceChangeAtDate(now.AddHours(-2), -30);

        user.Balance.ShouldBe(120);
        user.ReservedBalance.ShouldBe(0);

        // Verify sorted order
        user.Transactions[0].When.ShouldBe(now.AddHours(-3));
        user.Transactions[0].Balance.ShouldBe(100);

        user.Transactions[1].When.ShouldBe(now.AddHours(-2));
        user.Transactions[1].Balance.ShouldBe(70);

        user.Transactions[2].When.ShouldBe(now.AddHours(-1));
        user.Transactions[2].Balance.ShouldBe(120);
    }

    [Fact]
    public void RecalculateBalanceAndReserve_FutureNegativeTransactions_CalculatesReservedBalance() {
        var user = new UserRecord();
        var now = DateTime.UtcNow;

        user.SetBalanceChangeAtDate(now.AddHours(-2), 200);
        user.SetBalanceChangeAtDate(now.AddDays(1), -50);
        user.SetBalanceChangeAtDate(now.AddDays(2), -30);
        user.SetBalanceChangeAtDate(now.AddDays(3), 100);

        user.Balance.ShouldBe(200);
        user.ReservedBalance.ShouldBe(80);

        user.Transactions[0].Balance.ShouldBe(200);
        user.Transactions[1].Balance.ShouldBe(150);
        user.Transactions[2].Balance.ShouldBe(120);
        user.Transactions[3].Balance.ShouldBe(220);
    }

    [Fact]
    public void HasSufficientFunds_ChecksAvailableBalanceAgainstReserved() {
        var user = new UserRecord();
        var now = DateTime.UtcNow;

        user.SetBalanceChangeAtDate(now.AddHours(-2), 200);
        user.SetBalanceChangeAtDate(now.AddDays(1), -80);

        user.HasSufficientFunds(100).ShouldBeTrue();
        user.HasSufficientFunds(120).ShouldBeTrue();
        user.HasSufficientFunds(121).ShouldBeFalse();
    }
}
