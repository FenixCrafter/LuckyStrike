using Microsoft.AspNetCore.Identity;

namespace WebApi.Entities;

public class User: IdentityUser
{
    public string? RefreshToken { get; set; }
    public DateTime RefreshTokenExpiryTime { get; set; }
    public string FullName { get; set; }
    public string? Street { get; set; }
    public string? City { get; set; }
    public string? Zipcode { get; set; }
    public string? EmailConfirmToken { get; set; }
    public string? PasswordResetToken { get; set; }
}