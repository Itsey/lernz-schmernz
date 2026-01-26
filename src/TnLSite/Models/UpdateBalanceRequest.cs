namespace TnLSite.Models;

public sealed class UpdateBalanceRequest {
    public string UserId { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public DateTime Date { get; set; }
}
