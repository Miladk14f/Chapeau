using Chapeau.Models;

namespace Chapeau.ViewModels
{
    public class OrderViewModel
    {
        public List<MenuItem> MenuItems;          // FULL menu — only used to build the filter buttons
        public List<MenuItem> FilteredMenuItems;   // the subset actually shown as rows
        public List<OrderItem> OrderItems;
        public RestaurantTable TableOrder;
        public Staff StaffOrder;
        public string SelectedCategory;
        public string SelectedSubCategory;


        public OrderViewModel(List<MenuItem> menuItems, List<MenuItem> filteredMenuItems, List<OrderItem> orderItems,
            RestaurantTable tableOrder, Staff staffOrder, string selectedCategory, string selectedSubCategory)
        {
            MenuItems = menuItems;
            FilteredMenuItems = filteredMenuItems;
            OrderItems = orderItems;
            TableOrder = tableOrder;
            StaffOrder = staffOrder;
            SelectedCategory = selectedCategory;
            SelectedSubCategory = selectedSubCategory;
        }

        public decimal OrderTotal => OrderItems.Sum(i => i.Price * i.Qty);
        public int OrderCount => OrderItems.Sum(i => i.Qty);
    }
}