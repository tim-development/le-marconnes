using LeMarconnes.Data;
using LeMarconnes.Entities;
using LeMarconnesAPI.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LeMarconnes.Tests.Helpers;

public static class ControllerFactory
{
    public static AccommodationsController Create(
        ApplicationDbContext context,
        UserManager<User> userManager,
        bool isAdmin)
    {
        var controller = new AccommodationsController(context, userManager);

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, "test-user")
        };

        if (isAdmin)
        {
            claims.Add(new Claim(ClaimTypes.Role, "Admin"));
        }

        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(new ClaimsIdentity(claims, "TestAuth"))
            }
        };

        return controller;
    }
}
