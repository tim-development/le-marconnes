using LeMarconnes.DTOs;

namespace LeMarconnes.Models
{
    public class AccommodationDetailViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int Capacity { get; set; }
        public decimal RatePerNight { get; set; }
        public bool IsHotelRoom { get; set; }
        public bool IsGite { get; set; }

        public int? RoomNumber { get; set; }
        public bool PrivateBathroom { get; set; }

        public bool EntireProperty { get; set; }
        public bool Garden { get; set; }
        public bool ParkingAvailable { get; set; }

        public List<AccommodationBedDto> AccommodationBeds { get; set; } = new();
    }
}