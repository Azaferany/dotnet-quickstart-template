using System.Security.Claims;

namespace QuickstartTemplate.ApplicationCore.Common;

public static class IdentityExtensions
{
    public static string UserId(this ClaimsPrincipal user)
    {

        if (user.Identity?.IsAuthenticated == true)

            return user.FindFirst("sub")?.Value;

        else
            return string.Empty;
    }

}