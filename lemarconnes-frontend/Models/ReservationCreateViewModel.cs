using System.ComponentModel.DataAnnotations;

namespace LeMarconnes.Models
{
    public class ReservationCreateViewModel
    {
        public int AccommodationId { get; set; }
        public string AccommodationName { get; set; } = string.Empty;
        public decimal RatePerNight { get; set; }
        public int MaxCapacity { get; set; }

        public bool IsGite { get; set; }
        public bool IsHotelRoom { get; set; }

        public string BlockedDatesJson { get; set; } = "[]";

        [Required(ErrorMessage = "Selecteer een duur.")]
        public int Duration { get; set; } = 3; // Helper: 3, 4, 7 of 0 (Custom)

        [Required(ErrorMessage = "Aantal gasten is verplicht.")]
        [Range(1, 25, ErrorMessage = "Aantal gasten moet tussen 1 en 25 liggen.")]
        public int NumberOfGuests { get; set; } = 1;

        [Required(ErrorMessage = "Kies een aankomstdatum.")]
        [DataType(DataType.Date)]
        public DateTime CheckInDate { get; set; } = DateTime.Today.AddDays(1);

        [Required(ErrorMessage = "Kies een vertrekdatum.")]
        [DataType(DataType.Date)]
        public DateTime CheckOutDate { get; set; } = DateTime.Today.AddDays(4);

        public decimal TouristTax { get; set; } = 2.50m; 

        public string? DiscountCode { get; set; } 

        public decimal Discount { get; set; } = 0m; 

        [Required]
        public string PaymentMethod { get; set; } = "Full"; 
    }
}