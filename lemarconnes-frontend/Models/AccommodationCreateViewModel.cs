using System.ComponentModel.DataAnnotations;

namespace LeMarconnes.Models
{
    public class AccommodationCreateViewModel
    {
        [Required(ErrorMessage = "Naam is verplicht")]
        public string? Name { get; set; }

        [Required(ErrorMessage = "Beschrijving is verplicht")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Capaciteit is verplicht")]
        [Range(1, 100, ErrorMessage = "Capaciteit moet tussen 1 en 100 liggen")]
        public int Capacity { get; set; }

        [Required(ErrorMessage = "Prijs per nacht is verplicht")]
        public decimal RatePerNight { get; set; }

        public string SelectedType { get; set; } = "hotel";

        public int? RoomNumber { get; set; }
        public bool PrivateBathroom { get; set; }

        public bool EntireProperty { get; set; }
        public bool Garden { get; set; }
        public bool ParkingAvailable { get; set; }

        public List<BedSelectionItem> BedTypes { get; set; } = new();
    }

    public class BedSelectionItem
    {
        public int BedId { get; set; }
        public string? BedName { get; set; }
        public int Quantity { get; set; }
    }
}