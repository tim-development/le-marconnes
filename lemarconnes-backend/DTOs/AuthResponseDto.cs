namespace LeMarconnes.DTOs
{
    public class AuthResponseDto
    {
        public UserDto User { get; set; } = null!;
        public bool IsAdmin { get; set; }
        public string? Token { get; set; } // Voor toekomstige JWT implementatie
    }
}
