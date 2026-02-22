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
public class AccommodationsController : BaseController
{
    private readonly ApplicationDbContext _context;

    public AccommodationsController(ApplicationDbContext context, UserManager<User> userManager)
        : base(userManager)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<AccommodationDto>>> GetAccommodations()
    {
        return Ok(await GetAccommodationQuery().ToListAsync());
    }

    // GET: api/Accommodations/5 (Openbaar)
    [HttpGet("{id:int}")]
    public async Task<ActionResult<AccommodationDto>> GetAccommodationById(int id)
    {
        var result = await GetAccommodationQuery()
            .FirstOrDefaultAsync(a => a.Id == id);

        if (result == null) return NotFound();

        return Ok(result);
    }

    // POST: api/Accommodations/hotelroom
    [HttpPost("hotelroom")]
    [Authorize]
    public async Task<ActionResult<AccommodationDto>> AddHotelRoom(HotelRoom newHotelRoom)
    {
        if (!await IsAdmin()) return Forbid();

        newHotelRoom.IsHotelRoom = true;
        newHotelRoom.IsGite = false;

        _context.HotelRooms.Add(newHotelRoom);
        await _context.SaveChangesAsync();

        return await GetAccommodationById(newHotelRoom.Id);
    }

    // POST: api/Accommodations/gite
    [HttpPost("gite")]
    [Authorize]
    public async Task<ActionResult<AccommodationDto>> AddGite(Gite newGite)
    {
        if (!await IsAdmin()) return Forbid();

        newGite.IsHotelRoom = false;
        newGite.IsGite = true;

        _context.Gites.Add(newGite);
        await _context.SaveChangesAsync();

        return await GetAccommodationById(newGite.Id);
    }

    // PUT: api/Accommodations/hotelroom/5
    [HttpPut("hotelroom/{id:int}")]
    [Authorize]
    public async Task<IActionResult> UpdateHotelRoom(int id, HotelRoom updatedHotelRoom)
    {
        if (!await IsAdmin()) return Forbid();

        var hotelRoom = await _context.HotelRooms.FindAsync(id);
        if (hotelRoom == null) return NotFound();

        hotelRoom.Name = updatedHotelRoom.Name;
        hotelRoom.Description = updatedHotelRoom.Description;
        hotelRoom.Capacity = updatedHotelRoom.Capacity;
        hotelRoom.RatePerNight = updatedHotelRoom.RatePerNight;
        hotelRoom.RoomNumber = updatedHotelRoom.RoomNumber;
        hotelRoom.PrivateBathroom = updatedHotelRoom.PrivateBathroom;

        await _context.SaveChangesAsync();
        return NoContent();
    }

    // PUT: api/Accommodations/gite/5
    [HttpPut("gite/{id:int}")]
    [Authorize]
    public async Task<IActionResult> UpdateGite(int id, Gite updatedGite)
    {
        if (!await IsAdmin()) return Forbid();

        var gite = await _context.Gites.FindAsync(id);
        if (gite == null) return NotFound();

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

    // DELETE: api/Accommodations/5
    [HttpDelete("{id:int}")]
    [Authorize]
    public async Task<IActionResult> DeleteAccommodation(int id)
    {
        if (!await IsAdmin()) return Forbid();

        var accommodation = await _context.Accommodations.FindAsync(id);
        if (accommodation == null) return NotFound();

        _context.Accommodations.Remove(accommodation);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private IQueryable<AccommodationDto> GetAccommodationQuery()
    {
        return _context.Accommodations.Select(a => new AccommodationDto
        {
            Id = a.Id,
            Name = a.Name,
            Description = a.Description,
            Capacity = a.Capacity,
            RatePerNight = a.RatePerNight,
            IsHotelRoom = a.IsHotelRoom,
            IsGite = a.IsGite,

            AccommodationBeds = a.AccommodationBeds.Select(ab => new AccommodationBedDto
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