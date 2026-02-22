namespace LeMarconnes.DTOs
{
    public class AccommodationDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal RatePerNight { get; set; }
        public int Capacity { get; set; }
        public bool IsHotelRoom { get; set; }
        public bool IsGite { get; set; }


        public string? RoomNumber { get; set; }
        public bool PrivateBathroom { get; set; }


        public bool Garden { get; set; }
        public bool ParkingAvailable { get; set; }
        public bool EntireProperty { get; set; }


        public List<AccommodationBedDto> AccommodationBeds { get; set; } = new();
        public List<ReservationDto> Reservations { get; set; } = new();
    }
}