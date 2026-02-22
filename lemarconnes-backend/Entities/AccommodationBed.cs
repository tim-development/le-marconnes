using LeMarconnes.Entities;
using System.Text.Json.Serialization;

public class AccommodationBed
{
    public int Id { get; set; }
    public int AccommodationId { get; set; }
    public int BedId { get; set; }
    public int Quantity { get; set; }

    [JsonIgnore]
    // Voeg het vraagteken toe en verwijder = null!
    public Accommodation? Accommodation { get; set; }

    [JsonIgnore]
    // Voeg het vraagteken toe en verwijder = null!
    public Bed? Bed { get; set; }
}