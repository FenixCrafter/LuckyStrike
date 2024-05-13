using WebApi.Entities;

namespace WebApi.Dtos;

public class OccupiedLanesWithReservationIdDto
{
    public int LaneNumber { get; set; }
    public int ReservationId { get; set; }
    public List<LaneReservationTime> Timeframe { get; set; }
}
 