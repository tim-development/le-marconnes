using LeMarconnes.DTOs;
using LeMarconnes.Entities;
using LeMarconnes.Services;
using LeMarconnesAPI.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LeMarconnes.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : BaseController // Erft nu van BaseController
{
    private readonly SignInManager<User> _signInManager;
    private readonly IJwtService _jwtService;

    public AuthController(
        UserManager<User> userManager,
        SignInManager<User> signInManager,
        IJwtService jwtService) : base(userManager) // Geef userManager door naar BaseController
    {
        _signInManager = signInManager;
        _jwtService = jwtService;
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponseDto>> Login([FromBody] LoginDto dto)
    {
        var user = await _userManager.FindByEmailAsync(dto.Email);
        if (user == null) return Unauthorized(new { Message = "Inloggegevens incorrect" });

        var result = await _signInManager.CheckPasswordSignInAsync(user, dto.Password, false);

        if (result.Succeeded)
        {
            var roles = await _userManager.GetRolesAsync(user);
            var token = _jwtService.CreateToken(user, roles);

            return Ok(new AuthResponseDto
            {
                User = new UserDto
                {
                    Id = user.Id,
                    Name = user.Name,
                    Email = user.Email!,
                    Address = user.Address,
                    PhoneNumber = user.PhoneNumber,
                    IsAdmin = roles.Contains("Admin")
                },
                IsAdmin = roles.Contains("Admin"),
                Token = token
            });
        }

        return Unauthorized(new { Message = "Inloggegevens incorrect" });
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return Ok(new { Message = "Succesvol uitgelogd op de server." });
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto dto)
    {
        var user = new User
        {
            UserName = dto.Email,
            Email = dto.Email,
            Name = dto.Name,
            Address = dto.Address,
            PhoneNumber = dto.PhoneNumber
        };

        var result = await _userManager.CreateAsync(user, dto.Password);

        if (result.Succeeded)
        {
            // Bepaal de rol
            string role = "Customer";
            if (User.Identity?.IsAuthenticated == true && await IsAdmin())
            {
                role = dto.IsAdmin ? "Admin" : "Customer";
            }

            // Voeg de rol toe in de database
            await _userManager.AddToRoleAsync(user, role);

            // Haal de rollen opnieuw op voor een correcte response
            var actualRoles = await _userManager.GetRolesAsync(user);

            return Ok(new AuthResponseDto
            {
                User = new UserDto
                {
                    Id = user.Id,
                    Name = user.Name,
                    Email = user.Email!,
                    IsAdmin = actualRoles.Contains("Admin") // Dit komt nu echt uit de DB
                },
                IsAdmin = actualRoles.Contains("Admin"),
                Token = _jwtService.CreateToken(user, actualRoles)
            });
        }

        return BadRequest(result.Errors);
    }

    [HttpGet("me")]
    [Authorize]
    public async Task<ActionResult<UserDto>> GetCurrentUser()
    {
        var userIdentifier = User.FindFirstValue(ClaimTypes.NameIdentifier)
                             ?? User.FindFirstValue("sub");

        if (string.IsNullOrEmpty(userIdentifier))
        {
            return Unauthorized(new { Message = "Geen identifier gevonden in token." });
        }

        var user = await _userManager.FindByEmailAsync(userIdentifier)
                   ?? await _userManager.FindByIdAsync(userIdentifier);

        if (user == null)
        {
            return NotFound(new { Message = $"Gebruiker '{userIdentifier}' niet gevonden." });
        }

        var roles = await _userManager.GetRolesAsync(user);

        return Ok(new UserDto
        {
            Id = user.Id,
            Name = user.Name,
            Email = user.Email!,
            Address = user.Address,
            PhoneNumber = user.PhoneNumber,
            IsAdmin = roles.Contains("Admin")
        });
    }
}