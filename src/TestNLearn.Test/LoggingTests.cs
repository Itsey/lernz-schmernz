using System.Net;
using Flurl.Http;
using Plisky.Diagnostics;
using Shouldly;

namespace TestNLearn.Test;


public class LoggingTests {
    protected Bilge b = new("tnl-module1-test");


    [Fact]
    public async Task Get_user_without_logon_returns_unauthorised() {
        b.Info.Flow();

        var f = await "http://localhost:5050/api/web/user/1234".GetAsync();

        f.ResponseMessage.StatusCode.ShouldBeEquivalentTo(401);
    }


    [Fact(DisplayName = "Login>>Invalid>>Returns401")]
    public async Task Login_request_without_correct_creds_is_unauthorised() {
        b.Info.Flow();

        var f = await "http://localhost:5050/api/web/login/".PostJsonAsync(new {
            UserId = "123",
            Password = "456"
        });

        f.ResponseMessage.StatusCode.ShouldBeEquivalentTo(HttpStatusCode.Unauthorized);
    }


    [Fact]
    public async Task Weird_side_effects_get_executed_by_both_libraries() {
        b.Info.Flow();

        // Both loggers have issues with side effect code - in this case just a sleep, but where we add code in that does 
        // other stuff than just logging it gets executed during the logging.

        var f = await "http://localhost:5050/api/web/ping".GetAsync();
        f.ResponseMessage.StatusCode.ShouldBeEquivalentTo(HttpStatusCode.OK);

        f = await "http://localhost:5050/api/client/ping".GetAsync();
        f.ResponseMessage.StatusCode.ShouldBeEquivalentTo(HttpStatusCode.OK);
    }

}
