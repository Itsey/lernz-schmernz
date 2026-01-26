namespace TnLSite.Models;

public sealed class UserRecord {
    public string UserId { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public decimal Balance { get; set; }
    public bool Enabled { get; set; } = true;
    public DateTime LastLogin { get; set; }
}
