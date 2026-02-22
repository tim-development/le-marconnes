namespace LeMarconnes.DTOs;

public class HotelRoomDto : AccommodationDto
{
    public int? RoomNumber { get; set; }
    public bool? PrivateBathroom { get; set; }
}