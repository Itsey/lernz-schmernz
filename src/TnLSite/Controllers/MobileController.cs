using Microsoft.AspNetCore.Mvc;
using TnLSite.Models;
using TnLSite.Services;

namespace TnLSite.Controllers;

[ApiController]
[Route("api/mobile")]
public sealed class MobileController : ApiControllerBase {
    private readonly IAccountService accountService;
    private readonly ITokenService tokenService;

    public MobileController(IAccountService accountService, ITokenService tokenService) {
        this.accountService = accountService;
        this.tokenService = tokenService;
    }

    [HttpPost("login")]
    public ActionResult<string> Login([FromBody] LoginRequest request) {
        if (request is null) {
            return BadRequest();
        }

        var token = accountService.Login(request.UserId, request.Password);
        return token is null ? Unauthorized() : Ok(token);
    }

    [HttpGet("user/{userId}")]
    public ActionResult<UserDetails> GetUser(string userId) {
        if (!TryGetToken(out var token)) {
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
        if (!TryGetToken(out var token)) {
            return Unauthorized();
        }

        if (!tokenService.ValidateToken(userId, token)) {
            return Unauthorized();
        }

        var balance = accountService.GetBalance(userId, token);
        return balance is null ? NotFound() : Ok(balance.Value);
    }

    [HttpPost("user")]
    public ActionResult<UserDetails> CreateUser([FromBody] CreateUserRequest request) {
        if (request is null) {
            return BadRequest();
        }

        if (!TryGetToken(out var token)) {
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

        if (!TryGetToken(out var token)) {
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

    private bool TryGetToken(out string token) {
        token = Request.Headers[TOKEN_HEADER_NAME].ToString();
        return !string.IsNullOrWhiteSpace(token);
    }
}
