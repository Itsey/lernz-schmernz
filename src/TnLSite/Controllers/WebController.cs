using Microsoft.AspNetCore.Mvc;
using Plisky.Diagnostics;
using TnLSite.Models;
using TnLSite.Services;

namespace TnLSite.Controllers;

[ApiController]
[Route("api/web")]
public sealed class WebController : ApiControllerBase {
    protected Bilge b;
    private readonly AccountService accountService;
    private readonly ITokenService tokenService;

    public WebController(AccountService accountService, ITokenService tokenService, DynamicTrace dt) {
        b = dt.CreateBilge("tnl-web-controller");
        b.AddContext("controller", nameof(WebController));
        b.AddContext("flowstart", DateTime.Now.ToString("HH:mm:ss:fff"));

        b.Info.Flow("WebController");
        this.accountService = accountService;
        this.tokenService = tokenService;
    }





    [HttpPost("login")]
    public ActionResult<string> Login([FromBody] LoginRequest request) {
        b.Info.Flow();
        b.Info.Log("Quick Mesage To Test Logging");

        if (request is null) {
            b.Warning.Log("Null request found");
            return BadRequest();
        }

        if (string.IsNullOrWhiteSpace(request.UserId)) {
            b.Warning.Log("user id not set to a valid string");
            return BadRequest();
        }

        b.Info.Log($"attempting to authenticate {request.UserId}");
        string? token = accountService.Login(request.UserId, request.Password);
        return token is null ? Unauthorized() : Ok(token);
    }

    [HttpGet("ping")]
    public ActionResult<bool> Ping() {
        b.Info.Flow();

        b.Info.Log("Before");
        b.Verbose.Log($"Its {WeirdBuggySideEffectCode()}");
        b.Info.Log("After");
        return Ok(true);
    }

    [HttpGet("user/{userId}")]
    public ActionResult<UserDetails> GetUser(string userId) {
        b.Info.Flow($"{userId}");

        string? token = GetToken();
        if (token is null) {
            b.Warning.Log($"Request: {userId}. No token provided, failed to auth");
            return Unauthorized();
        }

        if (!tokenService.ValidateToken(userId, token)) {
            b.Warning.Log($"Request: {userId}. Token validation failed.");
            return Unauthorized();
        }

        var details = accountService.GetUserDetails(userId, token);

        if (details != null) {
            b.Info.Log($"Request: {userId}. User details retrieved successfully.");
        } else {
            b.Warning.Log($"Request: {userId}. User details not found.");
        }
        if (details == null) {
            b.Warning.Log($"Request: {userId}. User details not found.");
            return NotFound();
        }
        return Ok(details);
    }

    [HttpGet("balance/{userId}")]
    public ActionResult<decimal> GetBalance(string userId) {
        b.Info.Flow($"{userId}");

        string? token = GetToken();
        if (token is null) {
            b.Warning.Log($"Request: {userId}. No token provided, failed to auth");
            return Unauthorized();
        }

        if (!tokenService.ValidateToken(userId, token)) {
            b.Warning.Log($"Request: {userId}. Token validation failed.");
            return Unauthorized();
        }

        decimal? balance = accountService.GetBalance(userId, token);
        return balance is null ? NotFound() : Ok(balance.Value);
    }

    [HttpPost("user")]
    public ActionResult<UserDetails> CreateUser([FromBody] CreateUserRequest request) {
        b.Info.Flow();
        if (request is null) {
            return BadRequest();
        }

        string? token = GetToken();
        if (token is null) {
            b.Warning.Log($"Request: {request.UserId}. No token provided, failed to auth");
            return Unauthorized();
        }

        if (!tokenService.ValidateToken(request.UserId, token)) {
            b.Warning.Log($"Request: {request.UserId}. Token validation failed.");
            return Unauthorized();
        }

        var user = accountService.CreateUser(request.UserId, request.UserName, request.Password);
        return user is null ? BadRequest() : Ok(user);
    }

    [HttpPost("balance")]
    public ActionResult<decimal> UpdateBalance([FromBody] UpdateBalanceRequest request) {
        b.Info.Flow();
        if (request is null) {
            return BadRequest();
        }

        string? token = GetToken();
        if (token is null) {
            b.Warning.Log($"Request: {request.UserId}. No token provided, failed to auth");
            return Unauthorized();
        }

        if (!tokenService.ValidateToken(request.UserId, token)) {
            b.Warning.Log($"Request: {request.UserId}. Token validation failed.");
            return Unauthorized();
        }

        var result = accountService.UpdateBalance(request.UserId, token, request.Amount, request.Date);
        if (result.balance is null) {
            return BadRequest(result.error);
        }

        return Ok(result.balance.Value);
    }

    private string? GetToken() {
        b.Info.Flow();
        string token = Request.Headers[TOKEN_HEADER_NAME].ToString();
        return string.IsNullOrWhiteSpace(token) ? null : token;
    }
}
