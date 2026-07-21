using Plisky.Diagnostics;

namespace TnLSite.Repository;

public sealed class UserRecord {
    private readonly Bilge b = new Bilge("tnl-user-record");

    public List<TransactionRecord> Transactions { get; set; } = new List<TransactionRecord>();
    public string UserId { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public long Balance { get; set; }
    public long ReservedBalance { get; set; }
    public bool Enabled { get; set; } = true;
    public DateTime LastLogin { get; set; }

    internal bool HasSufficientFunds(long amount) {
        b.Info.Flow();
        return (Balance - ReservedBalance) >= amount;
    }

    internal void SetBalanceChangeAtDate(DateTime transferDate, long amount) {
        b.Info.Flow();
        Transactions.Add(new TransactionRecord() {
            Amount = amount,
            When = transferDate,
        });
        RecalculateBalanceAndReserve();
    }

    private void RecalculateBalanceAndReserve() {
        b.Info.Flow();
        if (Transactions == null || Transactions.Count == 0) {
            Balance = 0;
            ReservedBalance = 0;
            return;
        }

        Transactions.Sort((a, b) => a.When.CompareTo(b.When));

        long runningBalance = 0;
        long currentBalance = 0;
        long reserved = 0;
        DateTime now = DateTime.UtcNow;

        foreach (var tx in Transactions) {
            runningBalance += tx.Amount;
            tx.Balance = runningBalance;

            DateTime txTimeUtc = tx.When.Kind == DateTimeKind.Utc ? tx.When : tx.When.ToUniversalTime();

            if (txTimeUtc <= now) {
                currentBalance = runningBalance;
            } else {
                if (tx.Amount < 0) {
                    reserved += Math.Abs(tx.Amount);
                }
            }
        }

        Balance = currentBalance;
        ReservedBalance = reserved;
    }
}
