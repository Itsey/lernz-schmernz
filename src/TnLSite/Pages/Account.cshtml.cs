using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TnLSite.Services;

namespace TnLSite.Pages;

public sealed class AccountModel : PageModel {
    private readonly IAccountService accountService;

    public AccountModel(IAccountService accountService) {
        this.accountService = accountService;
    }

    [BindProperty(SupportsGet = true)]
    public string UserId { get; set; } = string.Empty;

    [BindProperty(SupportsGet = true)]
    public string Token { get; set; } = string.Empty;

    public string? UserName { get; set; }
    public decimal Balance { get; set; }

    [BindProperty]
    public decimal Amount { get; set; }

    [BindProperty]
    public DateTime Date { get; set; } = DateTime.Today;

    public string? Message { get; set; }

    public IActionResult OnGet() {
        return LoadDetails();
    }

    public IActionResult OnPost() {
        var result = accountService.UpdateBalance(UserId, Token, Amount, Date);
        if (result.balance is null) {
            Message = result.error ?? "Unable to update balance.";
        } else {
            Message = "Balance updated.";
        }

        return LoadDetails();
    }

    private IActionResult LoadDetails() {
        var details = accountService.GetUserDetails(UserId, Token);
        if (details is null) {
            Message ??= "Please log in again.";
            return Page();
        }

        UserName = details.UserName;
        Balance = details.Balance;
        return Page();
    }
}
