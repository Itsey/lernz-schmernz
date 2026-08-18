using System.Diagnostics.CodeAnalysis;
using CSharpFunctionalExtensions;
using Microsoft.AspNetCore.Mvc;
using Plisky.Diagnostics;
using TnLSite.Controllers;
using TnLSite.Models;
using TnLSite.Services;

namespace TnLSite.BilgeVersion;

[ApiController]
[Route("api/web")]
public sealed class WebController : ApiControllerBase {
    protected Bilge b;
    private readonly AccountService accountService;
    private readonly ITokenService tokenService;
    private readonly IPersistenceService persistenceService;

    public WebController(AccountService accountService, ITokenService tokenService, IPersistenceService persistenceService, DynamicTrace dt) {
        b = dt.CreateBilge("tnl-web-controller");
        b.AddContext("controller", nameof(WebController));
        b.AddContext("flowstart", DateTime.Now.ToString("HH:mm:ss:fff"));

        b.Info.Flow("WebController");
        this.accountService = accountService;
        this.tokenService = tokenService;
        this.persistenceService = persistenceService;
    }

    [HttpPost("user")]
    public ActionResult<UserDetails> CreateUser([FromBody] CreateUserRequest request) {
        b.Info.Flow();

        var v = ValidateRequestAndGetToken(request, request?.UserId);
        if (v.IsFailure) {
            return v.Error;
        }


        var user = accountService.CreateUser(request.UserId, request.UserName, request.Password);
        return user is null ? BadRequest() : Ok(user);
    }

    public ActionResult Gamble([FromBody] GambleRequest request) {
        return Ok();
    }

    [HttpGet("balance/{userId}")]
    public ActionResult<decimal> GetBalance(string userId) {
        b.Info.Flow($"{userId}");

        var v = ValidateRequestAndGetToken(userId, userId);
        if (v.IsFailure) {
            return v.Error;
        }

        decimal? balance = accountService.GetBalance(userId, v.Value);
        return balance is null ? NotFound() : Ok(balance.Value);
    }

    [HttpGet("user/{userId}")]
    public ActionResult<UserDetails> GetUser(string userId) {
        b.Info.Flow($"{userId}");

        var v = ValidateRequestAndGetToken(userId, userId);
        if (v.IsFailure) {
            b.Verbose.Log($"validation failed for {userId}, error.");
            return v.Error;
        }

        var details = accountService.GetUserDetails(userId, v.Value);

        if (details == null) {
            b.Warning.Log($"Request: {userId}. User details not found.");
            return NotFound();
        }

        b.Info.Log($"Request: {userId}. User details retrieved successfully.");
        return Ok(details);
    }

    public async Task<ActionResult> Gift([FromBody] GiftRequest request) {
        b.Info.Flow();

        var v = ValidateRequestAndGetToken(request, request?.UserId);
        if (v.IsFailure) {
            b.Verbose.Log($"validation failed for {request?.UserId}, error.");
            return v.Error;
        }

        ArgumentNullException.ThrowIfNull(request);
        ArgumentNullException.ThrowIfNull(request.UserId);
        ArgumentNullException.ThrowIfNull(request.RecipientId);
        ArgumentNullException.ThrowIfNull(request.Amount);
        ArgumentNullException.ThrowIfNull(request.TransferDate);

        await accountService.SendGift(request.UserId, request.RecipientId, request.Amount.Value, request.TransferDate.Value);

        return Ok();
    }

    [HttpPost("login")]
    public async Task<ActionResult<string>> Login([FromBody] LoginRequest request) {
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
        string? token = await accountService.Login(request.UserId, request.Password);
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

    [HttpPost("balance")]
    public ActionResult<decimal> UpdateBalance([FromBody] UpdateBalanceRequest request) {
        b.Info.Flow();

        var v = ValidateRequestAndGetToken(request, request?.UserId);
        if (v.IsFailure) {
            b.Verbose.Log($"validation failed for {request?.UserId}, error.");
            return v.Error;
        }

        ArgumentNullException.ThrowIfNull(request);

        var result = accountService.UpdateBalance(request.UserId, v.Value, request.Amount, request.Date);
        if (result.balance is null) {
            return BadRequest(result.error);
        }

        return Ok(result.balance.Value);
    }
    [HttpPost("/deposit")]
    public IActionResult Deposit(string userId, long amount) {
        b.Info.Flow();
        b.Info.Log($"[WEB] Deposit: User={userId}, Amount={amount}");

        var user = persistenceService.Deposit(userId, amount);

        b.Info.Log($"[WEB] Deposit complete: NewBalance={user.Balance}");

        return Ok(user);
    }

    private string? GetToken() {
        b.Info.Flow();
        string token = Request.Headers[TOKEN_HEADER_NAME].ToString();
        return string.IsNullOrWhiteSpace(token) ? null : token;
    }

    private Result<string, ActionResult> ValidateRequestAndGetToken([NotNullWhen(true)] object? request, [NotNullWhen(true)] string? userId) {
        b.Info.Flow();

        if (request is null || string.IsNullOrWhiteSpace(userId)) {
            b.Warning.Log("Request is null or userId is invalid.", userId);
            return Result.Failure<string, ActionResult>(BadRequest());
        }

        string? token = GetToken();
        if (token is null) {
            b.Warning.Log($"Request: {userId}. No token provided, failed to auth");
            return Result.Failure<string, ActionResult>(Unauthorized());
        }

        if (!tokenService.ValidateToken(userId, token)) {
            b.Warning.Log($"Request: {userId}. Token validation failed.");
            return Result.Failure<string, ActionResult>(Unauthorized());
        }

        return Result.Success<string, ActionResult>(token);
    }



}