namespace WebApi.Entities;

public class Order
{
    public int Id { get; set; }
    public string UserId { get; set; }
    public User User { get; set; }
    public int ReservationId { get; set; }
    public Reservation Reservation { get; set; }
    public double Price { get; set; }
    public List<ProductOrder> ProductOrders { get; set; }
    public DateTime TimeOrdered { get; set; }
    public bool IsReady { get; set; }
    public bool IsDelivered { get; set; }
    public DateTime TimeDelivered { get; set; }
}