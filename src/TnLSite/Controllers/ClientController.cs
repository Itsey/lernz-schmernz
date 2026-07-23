using Microsoft.AspNetCore.Mvc;
using TnLSite.Models;
using TnLSite.Services;

namespace TnLSite.Controllers;

[ApiController]
[Route("api/client")]
public sealed partial class ClientController : ApiControllerBase {

    [LoggerMessage(Level = LogLevel.Information, Message = "Method >> {Method}. {pmeter}")]
    static partial void LogFlowMessage(ILogger logger, string method, string pmeter = "");

    private readonly AccountService accountService;
    private readonly ILogger<ClientController> logger;
    private readonly ITokenService tokenService;

    public ClientController(AccountService accountService, ITokenService tokenService, ILogger<ClientController> logger) {
        this.accountService = accountService;
        this.tokenService = tokenService;
        this.logger = logger;
    }

    [HttpPost("login")]
    public async Task<ActionResult<string>> Login([FromBody] LoginRequest request) {
        LogFlowMessage(logger, nameof(Login));
        if (request is null) {
            logger.LogWarning("Login request was null.");
            return BadRequest();
        }

        string? token = await accountService.Login(request.UserId, request.Password);
        if (token is null) {
            logger.LogWarning("Login failed for user {UserId}.", request.UserId);
        }
        return token is null ? Unauthorized() : Ok(token);
    }


    [HttpGet("ping")]
    public ActionResult<bool> Ping() {
        LogFlowMessage(logger, nameof(Ping));

        logger.LogInformation("Before");
        logger.LogDebug($"Its {WeirdBuggySideEffectCode()}");
        logger.LogInformation("After");


        return Ok(true);
    }


    [HttpGet("user/{userId}")]
    public ActionResult<UserDetails> GetUser(string userId) {
        LogFlowMessage(logger, nameof(GetUser), userId);

        string? token = GetToken();
        if (token is null) {
            logger.LogWarning("Token missing for GetUser on {UserId}.", userId);
            return Unauthorized();
        }

        if (!tokenService.ValidateToken(userId, token)) {
            logger.LogWarning("Token invalid for GetUser on {UserId}.", userId);
            return Unauthorized();
        }

        var details = accountService.GetUserDetails(userId, token);
        if (details is null) {
            logger.LogWarning("User details not found for {UserId}.", userId);
        }
        return details is null ? NotFound() : Ok(details);
    }

    [HttpGet("balance/{userId}")]
    public ActionResult<decimal> GetBalance(string userId) {
        LogFlowMessage(logger, nameof(GetBalance), userId);

        string? token = GetToken();
        if (token is null) {
            logger.LogWarning("Token missing for GetBalance on {UserId}.", userId);
            return Unauthorized();
        }

        if (!tokenService.ValidateToken(userId, token)) {
            logger.LogWarning("Token invalid for GetBalance on {UserId}.", userId);
            return Unauthorized();
        }

        decimal? balance = accountService.GetBalance(userId, token);
        if (balance is null) {
            logger.LogWarning("Balance not found for {UserId}.", userId);
        }
        return balance is null ? NotFound() : Ok(balance.Value);
    }

    [HttpPost("user")]
    public ActionResult<UserDetails> CreateUser([FromBody] CreateUserRequest request) {
        LogFlowMessage(logger, nameof(CreateUser));

        if (request is null) {
            logger.LogWarning("CreateUser request was null.");
            return BadRequest();
        }

        string? token = GetToken();
        if (token is null) {
            logger.LogWarning("Token missing for CreateUser on {UserId}.", request.UserId);
            return Unauthorized();
        }

        if (!tokenService.ValidateToken(request.UserId, token)) {
            logger.LogWarning("Token invalid for CreateUser on {UserId}.", request.UserId);
            return Unauthorized();
        }

        var user = accountService.CreateUser(request.UserId, request.UserName, request.Password);
        if (user is null) {
            logger.LogWarning("CreateUser failed for {UserId}.", request.UserId);
        }
        return user is null ? BadRequest() : Ok(user);
    }

    [HttpPost("balance")]
    public ActionResult<decimal> UpdateBalance([FromBody] UpdateBalanceRequest request) {
        logger.LogInformation("Entered {Method}", nameof(UpdateBalance));
        if (request is null) {
            logger.LogWarning("UpdateBalance request was null.");
            return BadRequest();
        }

        string? token = GetToken();
        if (token is null) {
            logger.LogWarning("Token missing for UpdateBalance on {UserId}.", request.UserId);
            return Unauthorized();
        }

        if (!tokenService.ValidateToken(request.UserId, token)) {
            logger.LogWarning("Token invalid for UpdateBalance on {UserId}.", request.UserId);
            return Unauthorized();
        }

        var result = accountService.UpdateBalance(request.UserId, token, request.Amount, request.Date);
        if (result.balance is null) {
            logger.LogWarning("UpdateBalance failed for {UserId}. Error: {Error}", request.UserId, result.error);
            return BadRequest(result.error);
        }

        return Ok(result.balance.Value);
    }

    private string? GetToken() {
        logger.LogInformation("Entered {Method}", nameof(GetToken));
        string token = Request.Headers[TOKEN_HEADER_NAME].ToString();
        return string.IsNullOrWhiteSpace(token) ? null : token;
    }
}
