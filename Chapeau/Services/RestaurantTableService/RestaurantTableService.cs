using Chapeau.Models;
using Chapeau.Models.Enums;
using Chapeau.Repositories;
using Chapeau.ViewModels;

namespace Chapeau.Services
{
    public class RestaurantTableService : IRestaurantTableService
    {
        private readonly IRestaurantTableRepository _repository;
        private readonly IOrderRepository _orderRepository;

        public RestaurantTableService(IRestaurantTableRepository repository, IOrderRepository orderRepository)
        {
            _repository = repository;
            _orderRepository = orderRepository;
        }

        public List<RestaurantTable> GetAvailableTables()
        {
            List<RestaurantTable> tables = _repository.GetAllTables();
            List<RestaurantTable> available = new List<RestaurantTable>();
            foreach (RestaurantTable table in tables)
            {
                if (table.Status != TableStatus.Occupied)
                    available.Add(table);
            }
            return available;
        }

        public TableOverviewViewModel GetTableOverview()
        {
            List<RestaurantTable> tables = _repository.GetAllTables();
            List<Order> orders = _orderRepository.GetAllOrders();
            List<OrderItem> items = _orderRepository.GetAllOrderItems();

            Dictionary<int, int> tableIdByOrder = new Dictionary<int, int>();
            foreach (Order order in orders)
            {
                int tableId = order.Table != null ? order.Table.TableId : 0;
                tableIdByOrder[order.OrderId] = tableId;
            }

            Dictionary<int, TableCard> cardByTable = new Dictionary<int, TableCard>();
            foreach (RestaurantTable table in tables)
                cardByTable[table.TableId] = new TableCard { Table = table };

            foreach (OrderItem item in items)
            {
                if (item.Status != OrderItemStatus.Ready && item.Status != OrderItemStatus.InPreparation)
                    continue;

                int orderId = item.Order != null ? item.Order.OrderId : 0;
                if (!tableIdByOrder.ContainsKey(orderId))
                    continue;

                int tableId = tableIdByOrder[orderId];
                if (!cardByTable.ContainsKey(tableId))
                    continue;

                ReadyItemRow row = new ReadyItemRow { Name = item.Name, Qty = item.Qty };
                bool isDrink = Array.IndexOf(ItemTypeGroups.Drinks, item.ItemType) >= 0;

                if (item.Status == OrderItemStatus.Ready)
                {
                    if (isDrink)
                        cardByTable[tableId].ReadyDrink.Add(row);
                    else
                        cardByTable[tableId].ReadyFood.Add(row);
                }
                else
                {
                    if (isDrink)
                        cardByTable[tableId].PreparingDrink.Add(row);
                    else
                        cardByTable[tableId].PreparingFood.Add(row);
                }
            }

            TableOverviewViewModel vm = new TableOverviewViewModel();
            foreach (RestaurantTable table in tables)
                vm.Cards.Add(cardByTable[table.TableId]);

            return vm;
        }

        public RestaurantTable GetTableById(int id)
        {
            return _repository.GetTableById(id);
        }

        public void SeatGuestsAtTable(int tableId, int guests, int waiterId)
        {
            _repository.SeatGuestsAtTable(tableId, guests, waiterId);
        }

        public void ClearTable(int tableId)
        {
            _repository.ClearTable(tableId);
        }

        public void ReserveTable(int tableId, string reservationName, int guests, DateTime reservationAt)
        {
            _repository.ReserveTable(tableId, reservationName, guests, reservationAt);
        }
    }
}
