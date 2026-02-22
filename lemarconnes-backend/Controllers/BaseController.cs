using LeMarconnes.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LeMarconnesAPI.Controllers;

public class BaseController : ControllerBase
{
    protected readonly UserManager<User> _userManager;

    public BaseController(UserManager<User> userManager)
    {
        _userManager = userManager;
    }

    [NonAction]
    protected async Task<bool> IsAdmin()
    {
        var userIdentifier = User.FindFirstValue(ClaimTypes.NameIdentifier)
                             ?? User.FindFirstValue("sub");

        if (string.IsNullOrEmpty(userIdentifier)) return false;

        var user = await _userManager.FindByEmailAsync(userIdentifier)
                   ?? await _userManager.FindByIdAsync(userIdentifier);

        if (user == null) return false;

        // Check de rollen in de database
        var roles = await _userManager.GetRolesAsync(user);
        return roles.Contains("Admin");
    }
}