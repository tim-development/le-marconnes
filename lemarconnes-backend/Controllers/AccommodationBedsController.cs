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
public class AccommodationBedsController : BaseController
{
    private readonly ApplicationDbContext _context;

    public AccommodationBedsController(ApplicationDbContext context, UserManager<User> userManager)
        : base(userManager)
    {
        _context = context;
    }

    // GET: api/AccommodationBeds
    [HttpGet]
    public async Task<ActionResult<IEnumerable<AccommodationBedDto>>> GetAccommodationBeds()
    {
        return Ok(await GetMappedAccommodationBedsQuery().ToListAsync());
    }

    // GET: api/AccommodationBeds/5
    [HttpGet("{id:int}")]
    public async Task<ActionResult<AccommodationBedDto>> GetAccommodationBedById(int id)
    {
        var result = await GetMappedAccommodationBedsQuery()
            .FirstOrDefaultAsync(ab => ab.Id == id);

        if (result == null) return NotFound();

        return Ok(result);
    }

    // POST: api/AccommodationBeds
    [HttpPost]
    [Authorize]
    public async Task<ActionResult<AccommodationBedDto>> AddAccommodationBed(AccommodationBed newAccommodationBed)
    {
        if (!await IsAdmin()) return Forbid();

        _context.AccommodationBeds.Add(newAccommodationBed);
        await _context.SaveChangesAsync();

        return await GetAccommodationBedById(newAccommodationBed.Id);
    }

    // PUT: api/AccommodationBeds/5
    [HttpPut("{id:int}")]
    [Authorize]
    public async Task<IActionResult> UpdateAccommodationBed(int id, AccommodationBed updatedAccommodationBed)
    {
        if (!await IsAdmin()) return Forbid();

        var accommodationBed = await _context.AccommodationBeds.FindAsync(id);
        if (accommodationBed == null) return NotFound();

        accommodationBed.AccommodationId = updatedAccommodationBed.AccommodationId;
        accommodationBed.BedId = updatedAccommodationBed.BedId;
        accommodationBed.Quantity = updatedAccommodationBed.Quantity;

        await _context.SaveChangesAsync();
        return NoContent();
    }

    // DELETE: api/AccommodationBeds/5
    [HttpDelete("{id:int}")]
    [Authorize]
    public async Task<IActionResult> DeleteAccommodationBed(int id)
    {
        if (!await IsAdmin()) return Forbid();

        var accommodationBed = await _context.AccommodationBeds.FindAsync(id);
        if (accommodationBed == null) return NotFound();

        _context.AccommodationBeds.Remove(accommodationBed);
        await _context.SaveChangesAsync();
        return NoContent();
    }

    private IQueryable<AccommodationBedDto> GetMappedAccommodationBedsQuery()
    {
        return _context.AccommodationBeds
            .Select(ab => new AccommodationBedDto
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
                },

                Accommodation = null!,
            });
    }
}