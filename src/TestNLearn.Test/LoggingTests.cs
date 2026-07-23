using System.Net;
using Flurl.Http;
using Plisky.Diagnostics;
using Shouldly;

namespace TestNLearn.Test;


public class LoggingTests {
    protected Bilge b = new("tnl-module1-test");

    public LoggingTests() {

    }


    [Fact]
    public async Task Get_user_without_token_returns_unauthorised() {
        b.Info.Flow();
        // Was thinking that this was clearer but its actually rubbish.  
        await "http://localhost:5050/api/web/user/1234".AllowHttpStatus((int)HttpStatusCode.Unauthorized).GetAsync();
    }

    [Fact]
    public async Task Get_user_without_logon_returns_unauthorised() {
        b.Info.Flow();

        var f = await "http://localhost:5050/api/web/user/1234".AllowAnyHttpStatus().GetAsync();

        f.ResponseMessage.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }


    [Fact(DisplayName = "Login>>Invalid>>Returns401")]
    public async Task Login_request_without_correct_creds_is_unauthorised() {
        b.Info.Flow();

        var f = await "http://localhost:5050/api/web/login/".AllowAnyHttpStatus().PostJsonAsync(new {
            UserId = "123",
            Password = "456"
        });

        f.ResponseMessage.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }


    [Fact]
    public async Task Weird_side_effects_get_executed_by_both_libraries() {
        b.Info.Flow();

        // Both loggers have issues with side effect code - in this case just a sleep, but where we add code in that does 
        // other stuff than just logging it gets executed during the logging.

        var f = await "http://localhost:5050/api/web/ping".GetAsync();
        f.ResponseMessage.StatusCode.ShouldBe(HttpStatusCode.OK);

        f = await "http://localhost:5050/api/client/ping".GetAsync();
        f.ResponseMessage.StatusCode.ShouldBe(HttpStatusCode.OK);
    }


    [Fact]
    public async Task Usecase_1_user_logs_in_gets_balance() {
        b.Info.Flow();

        var f = await "http://localhost:5050/api/web/login/".AllowAnyHttpStatus().PostJsonAsync(new {
            UserId = "valid",
            Password = "valid"
        });

        f.ResponseMessage.StatusCode.ShouldBe(HttpStatusCode.OK);
    }

}
