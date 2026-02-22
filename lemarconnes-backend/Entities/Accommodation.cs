namespace LeMarconnes.Entities;

public class Accommodation
{
  public int Id { get; set; }
  public string Name { get; set; } = null!;
  public string Description { get; set; } = null!;
  public int Capacity { get; set; }
  public decimal RatePerNight { get; set; }
  public bool IsHotelRoom { get; set; }
  public bool IsGite { get; set; }

  public ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
  public ICollection<AccommodationBed> AccommodationBeds { get; set; } = new List<AccommodationBed>();
}
