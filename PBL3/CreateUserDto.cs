public class CreateUserDto
{
    public string username { get; set; }
    public string password { get; set; }
    public string fullName { get; set; }
    public string email { get; set; }
    // Dấu ? trong bảng nghĩa là có thể null
    public string? phoneNumber { get; set; }
    public string? address { get; set; }
    public DateTime? dateOfBirth { get; set; }
    public string role { get; set; }
}