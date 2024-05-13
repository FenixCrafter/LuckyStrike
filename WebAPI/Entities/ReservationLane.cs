namespace WebApi.Entities;

public class ReservationLane
{
    public int Id { get; set; }
    public int ReservationId { get; set; }
    public int LaneNumber { get; set; }
    public List<LaneReservationTime> Timeframe { get; set; }
}