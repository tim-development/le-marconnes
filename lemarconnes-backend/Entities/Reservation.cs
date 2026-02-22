namespace LeMarconnes.Entities;

public class Reservation
{
    public int Id { get; set; }


    public string UserId { get; set; } = null!;

    public int AccommodationId { get; set; }
    public int NumberOfGuests { get; set; }
    public decimal TouristTax { get; set; }
    public decimal Discount { get; set; }
    public DateTime CheckInDate { get; set; }
    public DateTime CheckOutDate { get; set; }
    public ReservationStatus Status { get; set; }

    public virtual User? User { get; set; }
    public virtual Accommodation? Accommodation { get; set; } 

    public ICollection<Payment> Payments { get; set; } = new List<Payment>();
}