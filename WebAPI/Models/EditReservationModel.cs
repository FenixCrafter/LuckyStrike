using WebApi.Entities;

namespace WebApi.Models;

public class EditReservationModel
{
    public DateTime ReservationDate { get; set; }
    public int Adults { get; set; }
    public int Children { get; set; }
    public bool IsCanceled { get; set; }
    public List<ReservationLane> Lanes { get; set; }
}