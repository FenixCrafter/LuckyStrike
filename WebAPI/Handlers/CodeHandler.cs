using Microsoft.EntityFrameworkCore;
using WebApi.Data;

namespace WebApi.Handlers;

public class CodeHandler
{
    private readonly ApplicationDbContext _context;
    private readonly DateHandler _dateHandler;
    public CodeHandler(ApplicationDbContext context, DateHandler dateHandler)
    {
        _context = context;
        _dateHandler = dateHandler;
    }

    private string GenerateCode()
    {
        return new Random().Next(1, 9999).ToString().PadLeft(4, '0');
    }

    private async Task<bool> IsCodeValid(string code)
    {
        var reservations = await _context.Reservations
            .Where(reservation => reservation.Code == code)
            .ToListAsync();

        reservations = reservations
            .Where(reservation => _dateHandler.GetUniqueDayKey(reservation.ReservationDate) == _dateHandler.GetUniqueDayKey(DateTime.Now))
            .ToList();

        return reservations.FirstOrDefault() == null;
    }
    
    public async Task<string> GetReservationCode()
    {
        string code;
        do
        {
            code = GenerateCode();
        } while (!await IsCodeValid(code));
        return code;
    }
}
