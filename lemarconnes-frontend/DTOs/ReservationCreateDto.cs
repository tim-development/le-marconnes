namespace LeMarconnes.DTOs
{
    public class ReservationCreateDto
    {
        public string UserId { get; set; } = string.Empty;
        public int AccommodationId { get; set; }
        public int NumberOfGuests { get; set; }
        public decimal TouristTax { get; set; }
        public decimal Discount { get; set; }
        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }
        public int Status { get; set; }
    }
}
