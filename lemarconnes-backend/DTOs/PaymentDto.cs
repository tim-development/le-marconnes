namespace LeMarconnes.DTOs;

public class PaymentDto
{
    public int Id { get; set; }
    public decimal Amount { get; set; }
    public DateTime PaymentDate { get; set; }

    public int ReservationId { get; set; }

    public ReservationDto Reservation { get; set; } = null!;
}