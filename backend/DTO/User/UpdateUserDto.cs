public class UpdateUserDto
{
    public string? fullName { get; set; }
    public string? email { get; set; }
    public string? phoneNumber { get; set; }
    public string? address { get; set; }
    public DateOnly? dateOfBirth { get; set; }
    public string? role { get; set; }
    public string? password { get; set; }
    public bool? isActive { get; set; }
}