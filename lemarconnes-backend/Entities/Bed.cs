using System.Text.Json.Serialization; // Voeg deze using toe

namespace LeMarconnes.Entities;

public class Bed
{
    public int Id { get; set; }
    public int numberOfPeople { get; set; }
    public string Type { get; set; } = null!;

    [JsonIgnore] 
    public ICollection<AccommodationBed> AccommodationBeds { get; set; } = new List<AccommodationBed>();
}