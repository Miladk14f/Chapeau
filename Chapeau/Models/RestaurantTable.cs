using Chapeau.Models.Enums;

namespace Chapeau.Models
{
    public class RestaurantTable
    {
        public int TableId { get; set; }
        public int Seats { get; set; }
        public int? Guests { get; set; }
        public ETableStatus Status { get; set; }
        public DateTime? SeatedAt { get; set; }
        public string ReservationName { get; set; }

        public Staff? Waiter { get; set; }

        public RestaurantTable() { }

        public RestaurantTable(int tableId, int seats, int? guests, ETableStatus status, DateTime? seatedAt, string reservationName)
        {
            TableId = tableId;
            Seats = seats;
            Guests = guests;
            Status = status;
            SeatedAt = seatedAt;
            ReservationName = reservationName;
        }
    }
}
