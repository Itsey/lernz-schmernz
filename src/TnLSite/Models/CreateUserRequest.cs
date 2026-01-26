namespace TnLSite.Models;

public sealed class CreateUserRequest {
    public string UserId { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}
