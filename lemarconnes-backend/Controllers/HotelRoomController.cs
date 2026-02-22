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
public class HotelRoomsController : BaseController // Erft nu van BaseController
{
    private readonly ApplicationDbContext _context;

    public HotelRoomsController(ApplicationDbContext context, UserManager<User> userManager)
        : base(userManager) // UserManager doorgeven aan de basis
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<HotelRoomDto>>> GetHotelRooms()
    {
        return Ok(await GetHotelRoomQuery().ToListAsync());
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<HotelRoomDto>> GetHotelRoomById(int id)
    {
        var result = await GetHotelRoomQuery().FirstOrDefaultAsync(h => h.Id == id);
        if (result == null) return NotFound();
        return Ok(result);
    }

    [HttpPost]
    [Authorize]
    public async Task<ActionResult<HotelRoomDto>> AddHotelRoom(HotelRoom newHotelRoom)
    {
        // Alleen admin check
        if (!await IsAdmin()) return Forbid();

        newHotelRoom.IsHotelRoom = true;
        newHotelRoom.IsGite = false;
        _context.HotelRooms.Add(newHotelRoom);
        await _context.SaveChangesAsync();

        return await GetHotelRoomById(newHotelRoom.Id);
    }

    [HttpPut("{id:int}")]
    [Authorize]
    public async Task<IActionResult> UpdateHotelRoom(int id, HotelRoom updatedRoom)
    {
        // Alleen admin check
        if (!await IsAdmin()) return Forbid();

        var hotelRoom = await _context.HotelRooms.FindAsync(id);
        if (hotelRoom == null) return NotFound();

        hotelRoom.Name = updatedRoom.Name;
        hotelRoom.Description = updatedRoom.Description;
        hotelRoom.Capacity = updatedRoom.Capacity;
        hotelRoom.RatePerNight = updatedRoom.RatePerNight;
        hotelRoom.RoomNumber = updatedRoom.RoomNumber;
        hotelRoom.PrivateBathroom = updatedRoom.PrivateBathroom;

        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    [Authorize]
    public async Task<IActionResult> DeleteHotelRoom(int id)
    {
        // Alleen admin check
        if (!await IsAdmin()) return Forbid();

        var hotelRoom = await _context.HotelRooms.FindAsync(id);
        if (hotelRoom == null) return NotFound();

        _context.HotelRooms.Remove(hotelRoom);
        await _context.SaveChangesAsync();
        return NoContent();
    }

    private IQueryable<HotelRoomDto> GetHotelRoomQuery()
    {
        return _context.HotelRooms.Select(h => new HotelRoomDto
        {
            Id = h.Id,
            Name = h.Name,
            Description = h.Description,
            Capacity = h.Capacity,
            RatePerNight = h.RatePerNight,
            IsHotelRoom = h.IsHotelRoom,
            IsGite = h.IsGite,
            RoomNumber = h.RoomNumber,
            PrivateBathroom = h.PrivateBathroom,

            AccommodationBeds = h.AccommodationBeds.Select(ab => new AccommodationBedDto
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