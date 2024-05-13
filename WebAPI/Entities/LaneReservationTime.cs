namespace WebApi.Entities;

public class LaneReservationTime
{
    public int Id { get; set; }
    public int ReservationLaneId { get; set; }
    public int Time { get; set; }
}