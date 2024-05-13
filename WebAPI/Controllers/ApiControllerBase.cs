using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

public abstract class ApiControllerBase : ControllerBase
{
    protected string? GetRequestUserId()
    {
        return User.Claims.FirstOrDefault(claim => claim.Type == "UserId")?.Value;
    }

    protected string? GetRefreshToken()
    {
        return User.Claims.SingleOrDefault(claim => claim.Type == "RefreshToken")?.Value;
    }
}