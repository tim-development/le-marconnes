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
public class ReservationController : BaseController // Erft nu van BaseController
{
    private readonly ApplicationDbContext _context;

    public ReservationController(ApplicationDbContext context, UserManager<User> userManager)
        : base(userManager) // UserManager doorgeven aan de basis
    {
        _context = context;
    }

    // GET: api/Reservation
    [HttpGet]
    [Authorize]
    public async Task<ActionResult<IEnumerable<ReservationDto>>> GetReservations()
    {
        // Optioneel: Je zou hier kunnen filteren zodat een 'Customer' alleen zijn eigen 
        // reserveringen ziet en een 'Admin' alles.
        return Ok(await GetReservationQuery().ToListAsync());
    }

    // GET: api/Reservation/5
    [HttpGet("{id:int}")]
    [Authorize]
    public async Task<ActionResult<ReservationDto>> GetReservationById(int id)
    {
        var result = await GetReservationQuery()
            .FirstOrDefaultAsync(r => r.Id == id);

        if (result == null) return NotFound();

        return Ok(result);
    }

    // POST: api/Reservation
    [HttpPost]
    [Authorize]
    public async Task<ActionResult<ReservationDto>> AddReservation(Reservation newReservation)
    {
        // Iedereen die ingelogd is mag een reservering aanmaken
        _context.Reservations.Add(newReservation);
        await _context.SaveChangesAsync();

        return await GetReservationById(newReservation.Id);
    }

    // PUT: api/Reservation/5
    [HttpPut("{id:int}")]
    [Authorize]
    public async Task<IActionResult> UpdateReservation(int id, Reservation updatedReservation)
    {
        // VEILIGHEID: Alleen een Admin mag bestaande reserveringen wijzigen 
        // (bijv. korting aanpassen of de status op 'Betaald' zetten)
        if (!await IsAdmin()) return Forbid();

        var reservation = await _context.Reservations.FindAsync(id);
        if (reservation == null) return NotFound();

        reservation.UserId = updatedReservation.UserId;
        reservation.AccommodationId = updatedReservation.AccommodationId;
        reservation.NumberOfGuests = updatedReservation.NumberOfGuests;
        reservation.CheckInDate = updatedReservation.CheckInDate;
        reservation.CheckOutDate = updatedReservation.CheckOutDate;
        reservation.TouristTax = updatedReservation.TouristTax;
        reservation.Discount = updatedReservation.Discount;
        reservation.Status = updatedReservation.Status;

        await _context.SaveChangesAsync();
        return NoContent();
    }

    // DELETE: api/Reservation/5
    [HttpDelete("{id:int}")]
    [Authorize]
    public async Task<IActionResult> DeleteReservation(int id)
    {
        // VEILIGHEID: Alleen een Admin mag reserveringen definitief verwijderen
        if (!await IsAdmin()) return Forbid();

        var reservation = await _context.Reservations.FindAsync(id);
        if (reservation == null) return NotFound();

        _context.Reservations.Remove(reservation);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private IQueryable<ReservationDto> GetReservationQuery()
    {
        return _context.Reservations.Select(r => new ReservationDto
        {
            Id = r.Id,
            UserId = r.UserId,
            User = new UserDto
            {
                Id = r.User.Id,
                Name = r.User.Name,
                Email = r.User.Email ?? string.Empty,
                PhoneNumber = r.User.PhoneNumber ?? string.Empty
            },
            AccommodationId = r.AccommodationId,

            Accommodation = new AccommodationDto
            {
                Id = r.Accommodation.Id,
                Name = r.Accommodation.Name,
                Description = r.Accommodation.Description,
                Capacity = r.Accommodation.Capacity,
                RatePerNight = r.Accommodation.RatePerNight,
                IsHotelRoom = r.Accommodation.IsHotelRoom,
                IsGite = r.Accommodation.IsGite,
                AccommodationBeds = r.Accommodation.AccommodationBeds.Select(ab => new AccommodationBedDto
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
            },

            TouristTax = r.TouristTax,
            Discount = r.Discount,

            CheckInDate = r.CheckInDate,
            CheckOutDate = r.CheckOutDate,
            NumberOfGuests = r.NumberOfGuests,
            Status = new ReservationStatusDto
            {
                Id = (int)r.Status,
                Name = r.Status.ToString()
            },

            Payments = r.Payments.Select(p => new PaymentDto
            {
                Id = p.Id,
                Amount = p.Amount,
                PaymentDate = p.PaymentDate,
                Reservation = null!
            }).ToList()
        });
    }
}