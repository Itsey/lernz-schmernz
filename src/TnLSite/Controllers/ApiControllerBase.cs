using Microsoft.AspNetCore.Mvc;

namespace TnLSite.Controllers;

public abstract class ApiControllerBase : ControllerBase {
    protected const string TOKEN_HEADER_NAME = "X-Auth-Token";

    public ApiControllerBase() {

    }

    public string WeirdBuggySideEffectCode() {
        string result = "Hello, World!";
        for (int i = 0; i < 10; i++) {
            result += $" {i}";
            Thread.Sleep(50);
        }
        return result;
    }
}
