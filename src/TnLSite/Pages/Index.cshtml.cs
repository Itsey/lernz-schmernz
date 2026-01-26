using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TnLSite.Services;

namespace TnLSite.Pages;

public sealed class IndexModel : PageModel {
    private readonly IAccountService accountService;

    public IndexModel(IAccountService accountService) {
        this.accountService = accountService;
    }

    [BindProperty]
    public string UserId { get; set; } = string.Empty;

    [BindProperty]
    public string Password { get; set; } = string.Empty;

    public string? ErrorMessage { get; set; }

    public IActionResult OnPost() {
        var token = accountService.Login(UserId, Password);
        if (token is null) {
            ErrorMessage = "Invalid user id or password.";
            return Page();
        }

        return RedirectToPage("Account", new { userId = UserId, token });
    }
}
