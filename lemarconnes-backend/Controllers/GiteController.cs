using LeMarconnes.Data;
using LeMarconnes.DTOs;
using LeMarconnes.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LeMarconnesAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class GitesController : BaseController // Erft nu van BaseController
{
    private readonly ApplicationDbContext _context;

    public GitesController(ApplicationDbContext context, UserManager<User> userManager)
        : base(userManager) // UserManager doorgeven naar de base
    {
        _context = context;
    }

    // GET: api/Gites
    [HttpGet]
    public async Task<ActionResult<IEnumerable<GiteDto>>> GetGites()
    {
        return Ok(await GetGiteQuery().ToListAsync());
    }

    // GET: api/Gites/5
    [HttpGet("{id:int}")]
    public async Task<ActionResult<GiteDto>> GetGiteById(int id)
    {
        var result = await GetGiteQuery().FirstOrDefaultAsync(g => g.Id == id);

        if (result == null)
        {
            return NotFound();
        }

        return Ok(result);
    }

    // POST: api/Gites
    [HttpPost]
    [Authorize]
    public async Task<ActionResult<GiteDto>> AddGite(Gite newGite)
    {
        // Alleen admins mogen gites toevoegen
        if (!await IsAdmin()) return Forbid();

        newGite.IsHotelRoom = false;
        newGite.IsGite = true;

        _context.Gites.Add(newGite);
        await _context.SaveChangesAsync();

        return await GetGiteById(newGite.Id);
    }

    // PUT: api/Gites/5
    [HttpPut("{id:int}")]
    [Authorize]
    public async Task<IActionResult> UpdateGite(int id, Gite updatedGite)
    {
        // Alleen admins mogen gites wijzigen
        if (!await IsAdmin()) return Forbid();

        var gite = await _context.Gites.FindAsync(id);

        if (gite == null)
        {
            return NotFound();
        }

        gite.Name = updatedGite.Name;
        gite.Description = updatedGite.Description;
        gite.Capacity = updatedGite.Capacity;
        gite.RatePerNight = updatedGite.RatePerNight;
        gite.EntireProperty = updatedGite.EntireProperty;
        gite.Garden = updatedGite.Garden;
        gite.ParkingAvailable = updatedGite.ParkingAvailable;

        await _context.SaveChangesAsync();

        return NoContent();
    }

    // DELETE: api/Gites/5
    [HttpDelete("{id:int}")]
    [Authorize]
    public async Task<IActionResult> DeleteGite(int id)
    {
        // Alleen admins mogen gites verwijderen
        if (!await IsAdmin()) return Forbid();

        var gite = await _context.Gites.FindAsync(id);
        if (gite == null)
        {
            return NotFound();
        }

        _context.Gites.Remove(gite);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private IQueryable<GiteDto> GetGiteQuery()
    {
        return _context.Gites.Select(g => new GiteDto
        {
            Id = g.Id,
            Name = g.Name,
            Description = g.Description,
            Capacity = g.Capacity,
            RatePerNight = g.RatePerNight,
            IsHotelRoom = g.IsHotelRoom,
            IsGite = g.IsGite,

            EntireProperty = g.EntireProperty,
            Garden = g.Garden,
            ParkingAvailable = g.ParkingAvailable,

            AccommodationBeds = g.AccommodationBeds.Select(ab => new AccommodationBedDto
            {
                Id = ab.Id,
                Quantity = ab.Quantity,
                AccommodationId = ab.AccommodationId,
                BedId = ab.BedId,
                Bed = new BedDto
                {
                    Id = ab.Bed.Id,
                    Type = ab.Bed.Type,
                    NumberOfPeople = ab.Bed.numberOfPeople
                }
            }).ToList()
        });
    }
}