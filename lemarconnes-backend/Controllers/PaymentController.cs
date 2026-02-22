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
public class PaymentController : BaseController
{
    private readonly ApplicationDbContext _context;

    public PaymentController(ApplicationDbContext context, UserManager<User> userManager)
        : base(userManager)
    {
        _context = context;
    }

    [HttpGet]
    [Authorize]
    public async Task<ActionResult<IEnumerable<PaymentDto>>> GetPayments()
    {
        return Ok(await GetPaymentQuery().ToListAsync());
    }

    [HttpGet("{id:int}")]
    [Authorize]
    public async Task<ActionResult<PaymentDto>> GetPaymentById(int id)
    {
        var result = await GetPaymentQuery()
            .FirstOrDefaultAsync(p => p.Id == id);

        if (result == null) return NotFound();

        return Ok(result);
    }

    [HttpPost]
    [Authorize]
    public async Task<ActionResult<PaymentDto>> AddPayment(Payment newPayment)
    {
        _context.Payments.Add(newPayment);
        await _context.SaveChangesAsync();

        return await GetPaymentById(newPayment.Id);
    }

    [HttpDelete("{id:int}")]
    [Authorize]
    public async Task<IActionResult> DeletePayment(int id)
    {
        if (!await IsAdmin()) return Forbid();

        var payment = await _context.Payments.FindAsync(id);
        if (payment == null) return NotFound();

        _context.Payments.Remove(payment);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private IQueryable<PaymentDto> GetPaymentQuery()
    {
        return _context.Payments.Select(p => new PaymentDto
        {
            Id = p.Id,
            Amount = p.Amount,
            PaymentDate = p.PaymentDate,
            ReservationId = p.ReservationId,

            Reservation = new ReservationDto
            {
                Id = p.Reservation.Id,
                UserId = p.Reservation.UserId,
                AccommodationId = p.Reservation.AccommodationId,
                NumberOfGuests = p.Reservation.NumberOfGuests,
                CheckInDate = p.Reservation.CheckInDate,
                CheckOutDate = p.Reservation.CheckOutDate,
                TouristTax = p.Reservation.TouristTax,
                Discount = p.Reservation.Discount,

                Status = new ReservationStatusDto
                {
                    Id = (int)p.Reservation.Status,
                    Name = p.Reservation.Status.ToString()
                },

                User = new UserDto
                {
                    Id = p.Reservation.User.Id,
                    Name = p.Reservation.User.Name,
                    Email = p.Reservation.User.Email ?? string.Empty,
                    Address = p.Reservation.User.Address ?? string.Empty,
                    PhoneNumber = p.Reservation.User.PhoneNumber ?? string.Empty
                },

                Accommodation = new AccommodationDto
                {
                    Id = p.Reservation.Accommodation.Id,
                    Name = p.Reservation.Accommodation.Name,
                    Description = p.Reservation.Accommodation.Description,
                    Capacity = p.Reservation.Accommodation.Capacity,
                    RatePerNight = p.Reservation.Accommodation.RatePerNight,
                    IsHotelRoom = p.Reservation.Accommodation.IsHotelRoom,
                    IsGite = p.Reservation.Accommodation.IsGite,

                    AccommodationBeds = p.Reservation.Accommodation.AccommodationBeds.Select(ab => new AccommodationBedDto
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
                        Accommodation = null! 
                    }).ToList()
                },

                Payments = null!
            }
        });
    }
}