using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LeMarconnes.Tests.Helpers;

public static class ControllerTestHelper
{
    public static void AttachAdminUser(ControllerBase controller)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, "admin-id"),
            new Claim(ClaimTypes.Role, "Admin")
        };

        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(
                    new ClaimsIdentity(claims, "TestAuth"))
            }
        };
    }

    public static void AttachNormalUser(ControllerBase controller)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, "user-id")
        };

        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(
                    new ClaimsIdentity(claims, "TestAuth"))
            }
        };
    }
}
