namespace LeMarconnes.DTOs;

public class UserDto
{
    public string Id { get; set; } = null!; // Identity User ID is een string (Guid)
    public string Name { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string PhoneNumber { get; set; } = null!;
    public string Address { get; set; } = null!;
    public bool IsAdmin { get; set; }
}