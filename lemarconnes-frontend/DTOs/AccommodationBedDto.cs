namespace LeMarconnes.DTOs
{
    public class AccommodationBedDto
    {
        public int Id { get; set; }
        public int AccommodationId { get; set; } 
        public int BedId { get; set; }           
        public int Quantity { get; set; }
        public BedDto Bed { get; set; } = new();
    }
}