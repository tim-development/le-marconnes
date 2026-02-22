public class ReservationDto
{
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

    [System.Text.Json.Serialization.JsonPropertyName("status")]
    public int Status { get; set; }

    [System.Text.Json.Serialization.JsonPropertyName("user")]
    public object? User { get; set; } = null;

    [System.Text.Json.Serialization.JsonPropertyName("accommodation")]
    public object? Accommodation { get; set; } = null;
}