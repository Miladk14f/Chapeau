using Chapeau.Models;

namespace Chapeau.ViewModels
{
    public class OrderViewModel
    {
        public List<MenuItem> MenuItems;
        public List<OrderItem> OrderItems;
        public RestaurantTable TableOrder;
        public Staff StaffOrder;

        public OrderViewModel(List<MenuItem> menuItems, List<OrderItem> orderItems, RestaurantTable tableOrder, Staff staffOrder)
        {
            MenuItems = menuItems;
            OrderItems = orderItems;
            TableOrder = tableOrder;
            StaffOrder = staffOrder;
        }
    }
}

