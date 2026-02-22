namespace LeMarconnes.Models
{
    public class AccommodationEditViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int Capacity { get; set; }
        public decimal RatePerNight { get; set; }
        public string SelectedType { get; set; } = string.Empty; 

        public int? RoomNumber { get; set; }
        public bool PrivateBathroom { get; set; }

        public bool EntireProperty { get; set; }
        public bool Garden { get; set; }
        public bool ParkingAvailable { get; set; }

        public List<BedSelectionItem> BedTypes { get; set; } = new();
    }
}