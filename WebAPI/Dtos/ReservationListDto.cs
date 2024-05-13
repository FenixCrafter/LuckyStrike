using WebApi.Entities;

namespace WebApi.Dtos;

public class ReservationListDto
{
    public int Id { get; set; }
    public string UserId { get; set; }
    public string? UserName { get; set; }
    public DateTime ReservationDate { get; set; }
    public List<ReservationLane> Lanes { get; set; }
    public int Adults { get; set; }
    public int Children { get; set; }
    
}