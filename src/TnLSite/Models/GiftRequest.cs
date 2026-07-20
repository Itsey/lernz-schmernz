namespace TnLSite.Controllers;

public class GiftRequest : RequestBase {
    public string? UserId { get; set; }
    public string? RecipientId { get; set; }
    public long? Amount { get; set; }
    public DateTime? TransferDate { get; set; }
}
