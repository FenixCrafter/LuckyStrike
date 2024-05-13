namespace WebApi.Models;

public class RegisterModel
{
    public string Name { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public string? PhoneNumber { get; set; }
    public string? StreetName { get; set; }
    public string? City { get; set; }
    public string? ZipCode { get; set; }

}
