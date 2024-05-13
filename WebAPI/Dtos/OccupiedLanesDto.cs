using WebApi.Entities;

namespace WebApi.Dtos;

public class OccupiedLanesDto
{
    public int LaneNumber { get; set; }
    public List<LaneReservationTime> Timeframe { get; set; }
}