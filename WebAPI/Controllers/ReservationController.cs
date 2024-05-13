using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Policy;
using QuestPDF.Infrastructure;
using WebApi.Data;
using WebApi.DTO;
using WebApi.Dtos;
using WebApi.Entities;
using WebApi.Handlers;
using WebApi.Models;
using WebApi.Statics;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReservationController : ApiControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly PdfHandler _pdfHandler;
        private readonly UserHandler _userHandler;
        private readonly CodeHandler _codeHandler;
        
        public ReservationController(ApplicationDbContext context, UserHandler userHandler, IConfiguration configuration, PdfHandler pdfHandler, CodeHandler codeHandler)
        {
            _context = context;
            _configuration = configuration;
            _pdfHandler = pdfHandler;
            _userHandler = userHandler;
            _codeHandler = codeHandler;
        }
        
        [HttpPost("CreateReservation")]
        public async Task<ActionResult<Reservation>> CreateReservation(createReservationDto createReservationDto)
        {
            var userId = GetRequestUserId();
            
            //checks if user already has a reservation that day
            var checkIfUserHasReservationWithSameDate = await _context.Reservations
                .Where(x => x.ReservationDate.Date == createReservationDto.ReservationDate.Date && !x.IsCanceled)
                .Where(x => x.UserId == userId)
                .FirstOrDefaultAsync();
            
            if (checkIfUserHasReservationWithSameDate != null)
            {
                return BadRequest("Je hebt al een reservering op deze datum");

            }
            
            //checks if user exists, it should exist just a check
            var user = await _context.Users.FirstOrDefaultAsync(user => user.Id == userId);
            if (user == null)
                return BadRequest("Geen gebruiker gevonden");

            var reservation = new Reservation();
            reservation.ReservationDate = createReservationDto.ReservationDate;
            reservation.UserId = userId;
            reservation.Adults = createReservationDto.Adults;
            reservation.Children = createReservationDto.Children;
            reservation.IsCanceled = false;
            reservation.Code = await _codeHandler.GetReservationCode();
            reservation.Lanes = createReservationDto.Lanes;
            reservation.CreatedAt = DateTime.Now;
            reservation.UpdatedAt = DateTime.Now;
            
            _context.Reservations.Add(reservation);
         
            await _context.SaveChangesAsync();

            //email
            var body = $"Beste {user.FullName},<br />" +
                       $"<br />" +

                       $"Welkom bij Bowlingcentrum Luckystrike! We zijn verheugd dat je bij ons langs komt.<br />" +
                       $"<br />" +
                       $"Hier is je reserverings code: {reservation.Code}" +
                       $"<br />" +
                       $"Met deze code kunt u inloggen in ons horeca systeem om drinken of versnaperingen te bestellen, bewaar deze dus goed<br />" +
                       $"Met vriendelijke groet,<br />" +
                       $"Het Team van Bowlingcentrum Luckstrike<br /><br />";

            _userHandler.SendEmail("Bedankt voor het reserveren!", user.Email, body);

            return Ok(reservation);
        }

        //creates reservation with a userId, only used by employee and admin
        [Roles(Roles.ADMIN, Roles.EMPLOYEE)]
        [HttpPost("CreateReservationWithUser")]
        public async Task<ActionResult<Reservation>> CreateReservationWithUser(CreateReservationWithUserDto createReservationWithUserDto)
        {
            Reservation reservation = new Reservation();
            reservation.ReservationDate = createReservationWithUserDto.ReservationDate;
            reservation.UserId = createReservationWithUserDto.UserId;
            reservation.Adults = createReservationWithUserDto.Adults;
            reservation.Children = createReservationWithUserDto.Children;
            reservation.IsCanceled = false;
            reservation.Lanes = createReservationWithUserDto.Lanes;
            reservation.Code = await _codeHandler.GetReservationCode();
            reservation.CreatedAt = DateTime.Now;
            reservation.UpdatedAt = DateTime.Now;
            
            _context.Reservations.Add(reservation);
            await _context.SaveChangesAsync();
            return Ok(reservation);
        }
        
        [HttpGet("getPdf")]
        public async Task<ActionResult<Reservation>> getPdf(int reservationId)
        {
           await _pdfHandler.CreatePdf(reservationId);
            return Ok();
        }

        //gets occupied lanes for a specific date
        [HttpGet("GetOccupiedLanes")]
        public async Task<ActionResult<IEnumerable<OccupiedLanesDto>>> GetOccupiedLanes(DateTime date)
        {
            return await _context.Reservations
                .Where(x => x.ReservationDate.Date == date.Date  && !x.IsCanceled)
                .SelectMany(x => x.Lanes)
                .Include(lane => lane.Timeframe)
                .Select(lane => new OccupiedLanesDto
                {
                    LaneNumber = lane.LaneNumber,
                    Timeframe = lane.Timeframe
                })
                .ToListAsync();
        } 
        
        //gets occupied lanes by reservationId
        [Roles(Roles.ADMIN, Roles.EMPLOYEE)]
        [HttpGet("GetOccupiedLanesWithReservationId")]
        public async Task<List<OccupiedLanesWithReservationIdDto>> GetOccupiedLanesWithReservationId(DateTime date)
        {
            return await _context.Reservations
                .Where(x => x.ReservationDate.Date == date.Date  && !x.IsCanceled)
                .SelectMany(x => x.Lanes)
                .Include(lane => lane.Timeframe)
                .Select(lane => new OccupiedLanesWithReservationIdDto()
                {
                    LaneNumber = lane.LaneNumber,
                    Timeframe = lane.Timeframe,
                    ReservationId = lane.ReservationId
                })
                .ToListAsync();
        }
        
        
        [Roles(Roles.ADMIN, Roles.EMPLOYEE)]
        [HttpGet("GetReservations")]
        public async Task<List<ReservationListDto>> GetReservations()
        {
            return await _context.Reservations
                .Include(reservation => reservation.Lanes)
                .ThenInclude(lane => lane.Timeframe)
                .Where(reservation => !reservation.IsCanceled) // Exclude canceled reservations
                .Join(
                    _context.Users,
                    reservation => reservation.UserId,
                    user => user.Id,
                    (reservation, user) => new ReservationListDto
                    {
                        Id = reservation.Id,
                        UserId = reservation.UserId,
                        UserName = user.UserName,
                        ReservationDate = reservation.ReservationDate,
                        Lanes = reservation.Lanes,
                        Adults = reservation.Adults,
                        Children = reservation.Children
                    }
                )
                .ToListAsync(); }

        //get reservations from user that is logged in
        [HttpGet("GetUserReservations")]
        public async Task<ActionResult<IEnumerable<ReservationListDto>>> GetUserReservations()
        {
            string userId = GetRequestUserId();
            
            return await _context.Reservations
                .Include(reservation => reservation.Lanes)
                .ThenInclude(lane => lane.Timeframe)
                .Where(x => x.UserId == userId)
                .Where(reservation => !reservation.IsCanceled) // Exclude canceled reservations
                .Join(
                    _context.Users,
                    reservation => reservation.UserId,
                    user => user.Id,
                    (reservation, user) => new ReservationListDto
                    {
                        Id = reservation.Id,
                        UserId = reservation.UserId,
                        UserName = user.UserName,
                        ReservationDate = reservation.ReservationDate,
                        Lanes = reservation.Lanes,
                        Adults = reservation.Adults,
                        Children = reservation.Children
                    }
                )
                .ToListAsync();
        }
        
        [Roles(Roles.ADMIN, Roles.EMPLOYEE)]
        [HttpGet("GetReservationByDate")]
        public async Task<ActionResult<IEnumerable<ReservationListDto>>> GetReservationByDate(DateTime date)
        {
            return await _context.Reservations
                .Include(reservation => reservation.Lanes)
                .ThenInclude(lane => lane.Timeframe)
                .Where(x => x.ReservationDate.Date == date.Date)
                .Where(reservation => !reservation.IsCanceled) // Exclude canceled reservations
                .Join(
                    _context.Users,
                    reservation => reservation.UserId,
                    user => user.Id,
                    (reservation, user) => new ReservationListDto
                    {
                        Id = reservation.Id,
                        UserId = reservation.UserId,
                        UserName = user.UserName,
                        ReservationDate = reservation.ReservationDate,
                        Lanes = reservation.Lanes,
                        Adults = reservation.Adults,
                        Children = reservation.Children
                    }
                )
                .ToListAsync();
        }
        
        [HttpGet("GetReservation")]
        public async Task<ActionResult<Reservation>> GetReservation(int id)
        {
            Reservation reservation = await _context.Reservations
                .Include(reservation => reservation.Lanes)
                .ThenInclude(lane => lane.Timeframe)
                .FirstOrDefaultAsync(x => x.Id == id);
            
            if (reservation == null)
            {
                return NotFound("Geen reservering gevonden");
            }
            
            return Ok(reservation);
        }
        
        [HttpPut("CancelReservation")]
        public async Task<ActionResult<Reservation>> CancelReservation(int id)
        {
            Reservation reservation = await  _context.Reservations.FirstOrDefaultAsync(x => x.Id == id);
            
            if (reservation == null)
            {
                return NotFound("Geen reservering gevonden");
            }
            
            reservation.IsCanceled = true;
            reservation.UpdatedAt = DateTime.Now;
            
            await _context.SaveChangesAsync();
            return Ok(reservation);
        }
        
        [HttpPut("UpdateReservation")]
        public async Task<ActionResult<Reservation>> UpdateReservation(int id, EditReservationModel editReservationModel)
        {
            Reservation reservation = await _context.Reservations
                .Include(reservation => reservation.Lanes)
                .ThenInclude(lane => lane.Timeframe)
                .FirstOrDefaultAsync(x => x.Id == id);
            
            if (reservation == null)
            {
                return NotFound();
            }
            
            reservation.ReservationDate = editReservationModel.ReservationDate;
            reservation.Adults = editReservationModel.Adults;
            reservation.Children = editReservationModel.Children;
            reservation.Lanes = editReservationModel.Lanes;
            reservation.UpdatedAt = DateTime.Now;
            
            await _context.SaveChangesAsync();
            return Ok(reservation);
        }
    }
}
