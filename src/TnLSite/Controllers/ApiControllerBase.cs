using Microsoft.AspNetCore.Mvc;

namespace TnLSite.Controllers;

public abstract class ApiControllerBase : ControllerBase {
    protected const string TOKEN_HEADER_NAME = "X-Auth-Token";
}
