using Microsoft.AspNetCore.Mvc;
using Plisky.Diagnostics;
using TnLSite.Models;
using TnLSite.Services;

namespace TnLSite.Controllers;

[ApiController]
[Route("api/web")]
public sealed class WebController : ApiControllerBase {
    protected Bilge b;
    private readonly IAccountService accountService;
    private readonly ITokenService tokenService;

    public WebController(IAccountService accountService, ITokenService tokenService, DynamicTrace dt) {
        b = dt.CreateBilge("tnl-web-controller");

        b.Info.Flow();
        this.accountService = accountService;
        this.tokenService = tokenService;
    }

    [HttpPost("login")]
    public ActionResult<string> Login([FromBody] LoginRequest request) {
        b.Info.Flow();

        if (request is null) {
            b.Warning.Log("Null request found");
            return BadRequest();
        }

        if (string.IsNullOrWhiteSpace(request.UserId)) {
            b.Warning.Log("user id not set to a valid string");
            return BadRequest();
        }

        b.Info.Log($"attempting to authenticate {request.UserId}");
        var token = accountService.Login(request.UserId, request.Password);
        return token is null ? Unauthorized() : Ok(token);
    }

    [HttpGet("user/{userId}")]
    public ActionResult<UserDetails> GetUser(string userId) {
        b.Info.Flow();
        if (!TryGetToken(out var token)) {
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

        return details is null ? NotFound() : Ok(details);
    }

    [HttpGet("balance/{userId}")]
    public ActionResult<decimal> GetBalance(string userId) {
        b.Info.Flow();
        if (!TryGetToken(out var token)) {
            b.Warning.Log($"Request: {userId}. No token provided, failed to auth");
            return Unauthorized();
        }

        if (!tokenService.ValidateToken(userId, token)) {
            b.Warning.Log($"Request: {userId}. Token validation failed.");
            return Unauthorized();
        }

        var balance = accountService.GetBalance(userId, token);
        return balance is null ? NotFound() : Ok(balance.Value);
    }

    [HttpPost("user")]
    public ActionResult<UserDetails> CreateUser([FromBody] CreateUserRequest request) {
        b.Info.Flow();
        if (request is null) {
            return BadRequest();
        }

        if (!TryGetToken(out var token)) {
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

        if (!TryGetToken(out var token)) {
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

    private bool TryGetToken(out string token) {
        b.Info.Flow();
        token = Request.Headers[TOKEN_HEADER_NAME].ToString();
        return !string.IsNullOrWhiteSpace(token);
    }
}
