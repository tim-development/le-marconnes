namespace LeMarconnes.Entities;

public class Payment
{
  public int Id { get; set; }
  public int ReservationId { get; set; }
  public decimal Amount { get; set; }
  public DateTime PaymentDate { get; set; }
  
  public virtual Reservation? Reservation { get; set; }
}
