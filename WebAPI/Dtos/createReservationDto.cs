namespace WebApi.DTO;
using WebApi.Entities;

public class createReservationDto
{
    public DateTime ReservationDate { get; set; }
    public int Adults { get; set; }
    public int Children { get; set; }
    public List<ReservationLane> Lanes { get; set; }
}