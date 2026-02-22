using LeMarconnes.Entities; // Voor de enum

namespace LeMarconnes.DTOs;

public class ReservationDto
{
    public int Id { get; set; }
    public string UserId { get; set; } = null!;

    public int AccommodationId { get; set; }

    public int NumberOfGuests { get; set; }
    public DateTime CheckInDate { get; set; }
    public DateTime CheckOutDate { get; set; }

    public decimal TouristTax { get; set; }
    public decimal Discount { get; set; }

    public ReservationStatusDto Status { get; set; } = null!;

    public UserDto User{ get; set; } = null!;
    public AccommodationDto Accommodation { get; set; } = null!;

    public List<PaymentDto> Payments { get; set; } = new();
}