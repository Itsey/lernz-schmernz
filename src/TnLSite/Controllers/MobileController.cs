using Microsoft.AspNetCore.Mvc;
using TnLSite.Models;
using TnLSite.Services;
using Serilog;


namespace TnLSite.Controllers;

[ApiController]
[Route("api/mobile")]
public sealed class MobileController : ApiControllerBase {
    private readonly AccountService accountService;
    private readonly ITokenService tokenService;
    private readonly IPersistenceService persistenceService;
    

    public MobileController(AccountService accountService, ITokenService tokenService, IPersistenceService persistenceService, Serilog.ILogger logger) {
        this.accountService = accountService;
        this.tokenService = tokenService;
        this.persistenceService = persistenceService;
        Log.Logger = logger;
    }

    [HttpPost("login")]
    public async Task<ActionResult<string>> Login([FromBody] LoginRequest request) {
        Log.Information("[MOB] In Login method");
        if (request is null) {
            return BadRequest();
        }

        string? token = await accountService.Login(request.UserId, request.Password);
        return token is null ? Unauthorized() : Ok(token);
    }

    [HttpGet("user/{userId}")]
    public ActionResult<UserDetails> GetUser(string userId) {
        Log.Information($"[MOB] In GetUser method");
        string? token = GetToken();
        if (token is null) {
            return Unauthorized();
        }

        if (!tokenService.ValidateToken(userId, token)) {
            return Unauthorized();
        }

        var details = accountService.GetUserDetails(userId, token);
        return details is null ? NotFound() : Ok(details);
    }

    [HttpGet("balance/{userId}")]
    public ActionResult<decimal> GetBalance(string userId) {
        string? token = GetToken();
        if (token is null) {
            return Unauthorized();
        }

        if (!tokenService.ValidateToken(userId, token)) {
            return Unauthorized();
        }

        decimal? balance = accountService.GetBalance(userId, token);
        return balance is null ? NotFound() : Ok(balance.Value);
    }

    [HttpPost("user")]
    public ActionResult<UserDetails> CreateUser([FromBody] CreateUserRequest request) {
        if (request is null) {
            return BadRequest();
        }

        string? token = GetToken();
        if (token is null) {
            return Unauthorized();
        }

        if (!tokenService.ValidateToken(request.UserId, token)) {
            return Unauthorized();
        }

        var user = accountService.CreateUser(request.UserId, request.UserName, request.Password);
        return user is null ? BadRequest() : Ok(user);
    }

    [HttpPost("balance")]
    public ActionResult<decimal> UpdateBalance([FromBody] UpdateBalanceRequest request) {
        if (request is null) {
            return BadRequest();
        }

        string? token = GetToken();
        if (token is null) {
            return Unauthorized();
        }

        if (!tokenService.ValidateToken(request.UserId, token)) {
            return Unauthorized();
        }

        var result = accountService.UpdateBalance(request.UserId, token, request.Amount, request.Date);
        if (result.balance is null) {
            return BadRequest(result.error);
        }

        return Ok(result.balance.Value);
    }
    [HttpPost("/deposit")]
    public IActionResult Deposit(string userId, long amount) {
        Log.Information("[MOB] Deposit: User={UserId}, Amount={Amount}", userId, amount);

        var user = persistenceService.Deposit(userId, amount);

        Log.Information("[MOB] Deposit complete: NewBalance={Balance}", user.Balance);

        return Ok(user);
    }

    private string? GetToken() {
        string token = Request.Headers[TOKEN_HEADER_NAME].ToString();
        return string.IsNullOrWhiteSpace(token) ? null : token;
    }
}
