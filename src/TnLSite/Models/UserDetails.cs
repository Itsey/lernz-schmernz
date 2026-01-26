namespace TnLSite.Models;

public sealed class UserDetails {
    public string UserId { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public decimal Balance { get; set; }
    public bool Enabled { get; set; }
    public DateTime LastLogin { get; set; }
}
