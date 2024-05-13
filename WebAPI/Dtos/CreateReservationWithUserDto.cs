namespace WebApi.DTO;
using WebApi.Entities;

public class CreateReservationWithUserDto
{
    public string UserId { get; set; }
    public DateTime ReservationDate { get; set; }
    public int Adults { get; set; }
    public int Children { get; set; }
    public List<ReservationLane> Lanes { get; set; }
}
