namespace LeMarconnes.DTOs.Admin
{
    public class AdminReservationDto
    {
        [System.Text.Json.Serialization.JsonPropertyName("id")]
        public int Id { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("userId")]
        public string UserId { get; set; } = null!;

        [System.Text.Json.Serialization.JsonPropertyName("accommodationId")]
        public int AccommodationId { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("checkInDate")]
        public DateTime CheckInDate { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("checkOutDate")]
        public DateTime CheckOutDate { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("numberOfGuests")]
        public int NumberOfGuests { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("touristTax")]
        public decimal TouristTax { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("discount")]
        public decimal Discount { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("totalPrice")]
        public decimal TotalPrice { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("status")]
        public AdminReservationStatusDto? Status { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("accommodation")]
        public AdminAccommodationSummaryDto? Accommodation { get; set; }
    }

    public class AdminReservationStatusDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }

    public class AdminAccommodationSummaryDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal RatePerNight { get; set; } // Wordt gevuld via de extra API call
    }
}