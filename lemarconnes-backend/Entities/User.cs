using Microsoft.AspNetCore.Identity;


namespace LeMarconnes.Entities;

public class User : IdentityUser
{
  public string Name { get; set; } = null!;
  public string Address { get; set; } = null!;


  public ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
}
