using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApi.Data;
using WebApi.Dtos;
using WebApi.Entities;
using WebApi.Handlers;
using WebApi.Models;
using WebApi.Statics;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ApiControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly UserHandler _userHandler;
        private readonly DateHandler _dateHandler;
        private readonly IConfiguration _config;

        public UserController(UserManager<User> userManager, ApplicationDbContext context, UserHandler userHandler, DateHandler dateHandler, IConfiguration config)
        {
            _userManager = userManager;
            _context = context;
            _userHandler = userHandler;
            _dateHandler = dateHandler;
            _config = config;
        }

        [HttpPost("Login")]
        public async Task<ActionResult<JwtTokenDto>> Login(LoginModel model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                return Unauthorized("Email of wachtwoord is onjuist");
            }

            if (!user.EmailConfirmed)
            {
                return Unauthorized("Email is niet geverifieerd");
            }

            if (await _userManager.CheckPasswordAsync(user, model.Password))
            {
                return Ok(new JwtTokenDto() { Token = await _userHandler.Login(user) });
            }
            
            return Unauthorized("Email of wachtwoord is onjuist");

            
        }

        [HttpPost("ForgotPassword")]
        public async Task<ActionResult<JwtTokenDto>> ForgotPassword([FromBody] string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user != null && user.EmailConfirmed)
            {
                var token = Guid.NewGuid().ToString();
                user.PasswordResetToken = token;
                await _userManager.UpdateAsync(user);

                var url = $"http://localhost:5173/passwordReset/{token}";

                var body = $"Beste {user.FullName},<br />" +
                           $"<br />" +

                           $"<br />Je hebt aangegeven om je wachtwoord te resetten. Klik de volgende link om een nieuw wachtwoord in te voeren." +
                           $"<br />" +
                           $" <a href='{url}'>Klik hier!</a>" +
                           $"<br /><br />" +
                           $"Was jij dit niet? Dan kan je deze mail verwijderen." +
                           $"<br />" +
                           $"Met vriendelijke groet,<br />" +
                           $"Het Team van Bowlingcentrum Luckstrike<br /><br />" +
                           $"Werkt het linkje niet? Kopieer hem dan in je browser: {url}";

                _userHandler.SendEmail("Wachtwoord reset voor LuckyStrike", user.Email, body);

                return Ok();
            }
            return BadRequest("Email bestaat niet of is niet geverifieerd");
        }

        [HttpPost("NewPassword")]
        public async Task<ActionResult<JwtTokenDto>> NewPassword(NewPasswordModel model)
        {
            var user = await _context.Users.FirstOrDefaultAsync(user => user.PasswordResetToken == model.Token);
            if (user == null)
                return BadRequest("Email bestaat niet of is niet geverifieerd");

            var result = await _userManager.ResetPasswordAsync(user, await _userManager.GeneratePasswordResetTokenAsync(user), model.Password);

            if(result.Succeeded)
            {
                user.PasswordResetToken = null;
                await _context.SaveChangesAsync();
                return Ok();
            }
            return BadRequest("Er is iets fout gegaan, probeer het later opnieuw");
        }

        [HttpPost("ReservationCode/{code}")]
        public async Task<ActionResult<JwtTokenDto>> LoginWithReservationCode(string code)
        {
            var reservations = await _context.Reservations
                .Where(reservation => reservation.Code == code)
                .ToListAsync();

            var reservation = reservations
                .FirstOrDefault(reservation => _dateHandler.GetUniqueDayKey(reservation.ReservationDate) == _dateHandler.GetUniqueDayKey(DateTime.Now));

            if (reservation == null)
                return BadRequest("Code is onjuist, of reservation is niet voor vandaag");

            var user = await _context.Users.FirstOrDefaultAsync(user => user.Id == reservation.UserId);
            if (user == null)
                return Unauthorized("Deze code is niet gekoppeld aan een account");

            return Ok(new JwtTokenDto() { Token = await _userHandler.Login(user) });
        }
        
        [HttpPost("ReservationInvoice/{code}")]
        public async Task<ActionResult<ReservationIdDto>> GetPdfWithReservationCode(string code)
        {
            var reservations = await _context.Reservations
                .Where(reservation => reservation.Code == code)
                .ToListAsync();

            var reservation = reservations
                .FirstOrDefault(reservation => _dateHandler.GetUniqueDayKey(reservation.ReservationDate) == _dateHandler.GetUniqueDayKey(DateTime.Now));

            if (reservation == null)
                return BadRequest("Code is onjuist, of reservation is niet voor vandaag");

            var user = await _context.Users.FirstOrDefaultAsync(user => user.Id == reservation.UserId);
            if (user == null)
                return Unauthorized("Deze code is niet gekoppeld aan een account");
            
            await new PdfHandler(_config ,_context).CreatePdf(reservation.Id);

            return Ok(new ReservationIdDto() { ReservationId = reservation.Id });
        }

        [HttpPost("ConfirmEmail")]
        public async Task<ActionResult> ConfirmEmail(string token, string userId)
        {
            //Check if user exists
            var user = await _context.Users.SingleOrDefaultAsync(user => user.Id == userId);
            if (user == null)
                // if user is null return bad request
                return BadRequest("Gebruiker niet gevonden");

            //Check if token is valid for user
            var result = _userHandler.ConfirmEmail(user, token);
            if (!result.Result.Succeeded)
                // if token is invalid return bad request
                return BadRequest("Er is iets fout gegaan, probeer het later opnieuw");

            // if token is valid return ok
            return Ok();
        }
        
        [HttpPost("Register")]
        public async Task<ActionResult<string>> Register(Models.RegisterModel model)
        {
            //Check if user exists
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user != null)
                // if user exists return bad request
                return BadRequest("Email bestaat al");

            //Create user with role customer
            var  result =  await _userHandler.CreateUser(model, Roles.CUSTOMER);
            if (!result.Succeeded)
                // if user creation failed return bad request
                return BadRequest("Er is iets fout gegaan, probeer het later opnieuw");
            
            // if user creation succeeded return ok
            return Ok();
        }
        
        [Roles(Roles.ADMIN)]
        [HttpPost("RegisterEmployee")]
        public async Task<ActionResult<string>> RegisterEmployee(Models.RegisterModel model)
        {
            //Check if user exists
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user != null)
                // if user exists return bad request
                return BadRequest("Email bestaat al");

            //Create user with role employee
            var  result =  await _userHandler.CreateUser(model, Roles.EMPLOYEE);
            if (!result.Succeeded)
                // if user creation failed return bad request
                return BadRequest("Er is iets fout gegaan, probeer het later opnieuw");
            
            // if user creation succeeded return ok
            return Ok();
        }

        [Roles(Roles.ADMIN)]
        [HttpGet("Employees")]
        public async Task<ActionResult<User[]>> GetEmployees()
        {
            return Ok(_userManager.GetUsersInRoleAsync(Roles.EMPLOYEE).Result.ToArray());
        }

        [Roles(Roles.ADMIN)]
        [HttpPost("DeleteUser/{id}")]
        public async Task<ActionResult> DeleteUser(string id)
        {
            var user = await _context.Users.FirstOrDefaultAsync(user => user.Id == id);
            if (user == null)
                return NotFound("Gebruiker niet gevonden");

            _context.Users.Remove(user);

            await _context.SaveChangesAsync();

            return Ok();
        }

        [Roles(Roles.EMPLOYEE, Roles.ADMIN)]
        [HttpPost("CreatePartialUser")]
        public async Task<ActionResult<UserDto>> CreatePartialUser(PartialUserModel model)
        {
            var user = new User
            {
                FullName = model.UserName,
                Email = model.Email,
                UserName = model.UserName,
                PhoneNumber = model.PhoneNumber,
                SecurityStamp = Guid.NewGuid().ToString()
            };
            //create user
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            //add user to role
           var result = await _userManager.AddToRoleAsync(user, Roles.CUSTOMER);
              if (!result.Succeeded)
                  // if user creation failed return bad request
                return BadRequest("Er is iets fout gegaan, probeer het later opnieuw");
          //get user
           var createdUser = await _userManager.FindByEmailAsync(model.Email);

           var createdUserDto = new UserDto
           {
               Id = createdUser.Id,
               Email = createdUser.Email,
               FullName = createdUser.FullName,
               PhoneNumber = createdUser.PhoneNumber,
           };
          //return user if succeeded
           return Ok(createdUser);
        }

        [HttpPost("RefreshToken")]
        public async Task<ActionResult<JwtTokenDto?>> RefreshTokenAsync()
        {
            var userId = GetRequestUserId();
            if (userId == null)
                return BadRequest("Gebuiker niet gevonden");

            var refreshToken = GetRefreshToken();

            var user = _context.Users.SingleOrDefault(user => user.Id == userId);

            if (user == null || user.RefreshToken != refreshToken || user.RefreshTokenExpiryTime <= DateTime.Now)
                return null;

            return new JwtTokenDto() { Token = await _userHandler.Login(user) };
        }

        [HttpPost("RevokeToken")]
        [Authorize]
        public async Task<ActionResult> RevokeTokenAsync()
        {
            var userId = GetRequestUserId();
            if (userId == null)
                return BadRequest("Gebuiker niet gevonden");

            var user = _context.Users.SingleOrDefault(user => user.Id == userId);

            if (user == null)
                return BadRequest("Gebuiker niet gevonden");

            user.RefreshToken = null;

            await _context.SaveChangesAsync();

            return Ok();
        }
        
        [HttpGet("GetUser")]
        public async Task<ActionResult<UserDto>> GetUser(string userId)
        {
            // var userId = GetRequestUserId();
            if (userId == null)
                return BadRequest("Gebuiker niet gevonden");

            var user = await _context.Users.FindAsync(userId);

            if (user == null)
                return BadRequest("Gebuiker niet gevonden");
            
            UserDto userDto = new UserDto
            {
                Id = user.Id,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Street = user.Street,
                City = user.City,
                Zipcode = user.Zipcode,
                FullName = user.FullName,
            };

            return userDto;
        }
        
        [HttpGet("SearchUser")]
        public async Task<ActionResult<IEnumerable<UserDto>>> SearchUser(string search)
        {
            return await _context.Users
                .Where(user => user.FullName.Contains(search))
                .Select(user => new UserDto
                {
                    Id = user.Id,
                    Email = user.Email,
                    FullName = user.FullName,
                })
                .ToListAsync();
        }

    }
}