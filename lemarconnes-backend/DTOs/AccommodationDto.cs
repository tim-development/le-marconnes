namespace LeMarconnes.DTOs;

public class AccommodationDto
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public int Capacity { get; set; }
    public decimal RatePerNight { get; set; }
    public bool IsHotelRoom { get; set; }
    public bool IsGite { get; set; }

    public List<AccommodationBedDto> AccommodationBeds { get; set; } = new();
}