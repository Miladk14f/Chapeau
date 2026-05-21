using Chapeau.Models.Enums;

namespace Chapeau.Models
{
    public class RestaurantTable
    {
        public int Id { get; set; }
        public int Seats { get; set; }
        public int? Guests { get; set; }
        public ETableStatus Status { get; set; }
        public DateTime? SeatedAt { get; set; }
        public int? WaiterId { get; set; }
        public string ReservationName { get; set; }

        public Staff? Waiter { get; set; }

        public RestaurantTable() { }

        public RestaurantTable(int id, int seats, int? guests, ETableStatus status, DateTime? seatedAt, int? waiterId, string reservationName)
        {
            Id = id;
            Seats = seats;
            Guests = guests;
            Status = status;
            SeatedAt = seatedAt;
            WaiterId = waiterId;
            ReservationName = reservationName;
        }
    }
}
