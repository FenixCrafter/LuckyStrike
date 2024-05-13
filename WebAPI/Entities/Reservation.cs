using System.ComponentModel.DataAnnotations;

namespace WebApi.Entities;

public class Reservation
{
    public int Id { get; set; }
    public string UserId { get; set; }
    public DateTime ReservationDate { get; set; }
    public int Adults { get; set; }
    public int Children { get; set; }
    public bool IsCanceled { get; set; }
    [MaxLength(4)]
    public string Code { get; set; }
    public List<ReservationLane> Lanes { get; set; }
    public List<Order> Orders { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    
}