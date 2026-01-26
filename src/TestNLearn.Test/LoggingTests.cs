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


    [Fact(DisplayName = nameof(Login_request_without_correct_creds_is_unauthorised))]
    public async Task Login_request_without_correct_creds_is_unauthorised() {
        b.Info.Flow();

        var f = await "http://localhost:5050/api/web/login/".PostJsonAsync(new {
            UserId = "123",
            Password = "456"
        });

        f.ResponseMessage.StatusCode.ShouldBeEquivalentTo(401);
    }

}
