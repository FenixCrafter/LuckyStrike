using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Mail;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using WebApi.Entities;
using WebApi.Statics;

namespace WebApi.Handlers;

public class UserHandler
{
    private readonly IConfiguration _configuration;
    private readonly UserManager<User> _userManager;
    private readonly SmtpClient _smtpClient;

    public UserHandler(IConfiguration configuration, UserManager<User> userManager, SmtpClient smtpClient)
    {
        _configuration = configuration;
        _userManager = userManager;
        _smtpClient = smtpClient;
    }
    public async Task<string> Login(User user)
    {
        var userRoles = await _userManager.GetRolesAsync(user);
        
        // maak een nieuwe refresh token aan, deze wordt gebruikt om je access token (jwt) te vernieuwen
        var refreshToken = GenerateRefreshToken();

        // voeg claims toe aan de jwt token, deze worden gebruikt om de gebruiker te identificeren en kunnen er later weer uitgehaald worden
        var claims = new List<Claim>
        {
            new Claim("UserId", user.Id),
            new Claim("UserName", user.FullName),
            new Claim("RefreshToken", refreshToken)
        };

        // voeg de rollen toe aan de jwt token met een claim type van "Role".
        // dit claim type zorgt er voor dat de rollen kunnen worden gecheckt in de Authorize(Roles) attribute
        // ook kunnen de rollen er weer uitgehaald worden in de frontend om te checken tot welke paginas de gebruiker toegang heeft
        foreach (var userRole in userRoles)
        {
            claims.Add(new Claim(ClaimTypes.Role, userRole));
        }

        // sla de refresh token op in de database
        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiryTime = DateTime.Now.AddDays(_configuration.GetValue<int>("JWT:RefreshTokenValidityInDays"));
        await _userManager.UpdateAsync(user);

        // maak een nieuwe token descriptor aan, deze wordt gebruikt om de jwt token te genereren
        // hier worden de claims in gezet, de secret key, de geldigheidsduur en het encryptie algoritme
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(_configuration.GetValue<int>("JWT:TokenValidityInMinutes")),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetValue<string>("JWT:Secret"))), SecurityAlgorithms.HmacSha256)
        };
        var tokenHandler = new JwtSecurityTokenHandler();
        var securityToken = tokenHandler.CreateToken(tokenDescriptor);
        var token = tokenHandler.WriteToken(securityToken);

        // geeft het token terug aan de frontend
        return token;
    }

    // maakt een random string
    public string GenerateRefreshToken()
    {
        var randomNumber = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        // vul de byte array met random bytes van lengte 64
        rng.GetBytes(randomNumber);
        // zet de byte array om naar een base64 string (maakt characters van de bytes)
        return Convert.ToBase64String(randomNumber);
    }
    
    public async Task<IdentityResult> CreateUser(Models.RegisterModel model, string role)
    {
        if (!IsValidEmail(model.Email))
            return IdentityResult.Failed(new IdentityError() { Description = "Email bestaat niet" });
        
        //generate token
        var confirmToken = Guid.NewGuid().ToString();
        var user = new User()
        {
            Email = model.Email,
            UserName = model.Email,
            FullName = model.Name,
            SecurityStamp = Guid.NewGuid().ToString(),
            City = model.City,
            Street = model.StreetName,
            Zipcode = model.ZipCode,
            PhoneNumber = model.PhoneNumber,
            EmailConfirmToken = confirmToken
        };
        //create user
        var result = await _userManager.CreateAsync(user, model.Password);
            
        if (!result.Succeeded)
            //if user creation failed return result
            return result;
        //if user creation succeeded send email if user is not employee or admin
        if (role == Roles.EMPLOYEE || role == Roles.ADMIN)
        {
            //if user is employee or admin confirm email automatically
            user.EmailConfirmed = true;
            await _userManager.UpdateAsync(user);
        }
        else
        {
            var url = $"http://localhost:5173/confirm/{confirmToken}/{user.Id}";
            var body = $"Beste {model.Name},<br />" +
                       $"<br />" +

                       $"Welkom bij Bowlingcentrum Luckystrike! We zijn verheugd dat je deel uitmaakt van onze bowlinggemeenschap. Om te beginnen en toegang te krijgen tot alle opwindende functies en voordelen, hoef je alleen maar je account te activeren.<br />" +
                       $"<br />" +
                       $" <a href='{url}'>Klik hier!</a>" +
                       $"<br />" +
                       $"Als je tijdens het activeringsproces problemen ondervindt of vragen hebt, neem dan gerust contact op met onze medewerkers op ons telefoon nummer 061212121212.<br /> We staan klaar om je te helpen bij elke stap.<br />" +
                       $"Bedankt dat je Bowlingcentrum luckystrike hebt gekozen. We kijken ernaar uit je op de banen te zien voor een geweldige tijd!<br />" +
                       $"<br />" +
                       $"Met vriendelijke groet,<br />" +
                       $"Het Team van Bowlingcentrum Luckstrike<br /><br />" +
                       $"Werkt het linkje niet? Kopieer hem dan in je browser: {url}";
            //send confirmation email
            SendEmail("Verifieer uw email!", model.Email,body);       
        }
        
        //add role to user
        await _userManager.AddToRoleAsync(user, role);
            //return result
        return result;
    }

    //check if email is valid
    static bool IsValidEmail(string email)
    {
        // Define the regular expression pattern for a valid email address
        string pattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";

        // Create a Regex object with the pattern
        Regex regex = new Regex(pattern);

        // Use the regex object to match the email against the pattern
        Match match = regex.Match(email);

        // Return true if the email matches the pattern, otherwise false
        return match.Success;
    }
    
    public async void SendEmail(string subject, string email, string body)
    {
        try
        {
            //get email
            var fromMail = _configuration["Mail:Email"];
            //create MailMessage object
            using (var message = new MailMessage(fromMail, email)
            {
                Subject = subject,
                Body = body,
                IsBodyHtml = true,
            })

            //send email using smtpClient
            await _smtpClient.SendMailAsync(message);
        }
        catch(Exception e)
        {
          Console.WriteLine(e);
        }
    }
    
    public Task<IdentityResult> ConfirmEmail(User user, string token)
    {
        var confirmToken = user.EmailConfirmToken;
        if (confirmToken == token)
        {
            user.EmailConfirmed = true;
           return _userManager.UpdateAsync(user);
        }
        return null;
    }
}
