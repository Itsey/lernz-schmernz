namespace TnLSite.Repository;

public class TransactionRecord {
    public TransactionRecord() { }

    public long Amount { get; set; }
    public DateTime When { get; set; }

    public long Balance { get; set; }
}