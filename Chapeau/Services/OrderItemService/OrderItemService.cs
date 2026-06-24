using Chapeau.Models;
using Chapeau.Models.Enums;
using Chapeau.Repositories;

namespace Chapeau.Services
{
    public class OrderItemService : IOrderItemService
    {
        private readonly IOrderItemRepository _repository;
        private readonly IOrderRepository _orderRepository;
        private readonly IMenuItemRepository _menuItemRepository;
        private readonly IRestaurantTableRepository _tableRepository;

        public OrderItemService(IOrderItemRepository repository, IOrderRepository orderRepository, IMenuItemRepository menuItemRepository, IRestaurantTableRepository tableRepository)
        {
            _repository = repository;
            _orderRepository = orderRepository;
            _menuItemRepository = menuItemRepository;
            _tableRepository = tableRepository;
        }

        public void UpdateOrderItemStatus(int id, Models.Enums.OrderItemStatus status)
        {
            _repository.UpdateOrderItemStatus(id, status);
        }

        public void UpdateOrderItemNote(int id, string note)
        {
            note = string.IsNullOrWhiteSpace(note) ? null : note.Trim();
            _repository.UpdateOrderItemNote(id, note);
        }

        public void StartPreparingItems(int orderId, ItemType[] types)
        {
            _repository.UpdateOrderItemsStatusByType(orderId, types, OrderItemStatus.Ordered, OrderItemStatus.InPreparation);
        }

        public void MarkOrderItemsReady(int orderId, ItemType[] types)
        {
            _repository.UpdateOrderItemsStatusByType(orderId, types, OrderItemStatus.InPreparation, OrderItemStatus.Ready);
        }

        public void AddItemToTable(int menuItemId, int tableId)
        {
            Order order = _orderRepository.GetActiveOrderByTableId(tableId);
            List<OrderItem> items = _repository.GetOrderItemsByTableId(tableId);

            OrderItem existing = null;
            foreach (OrderItem item in items)
            {
                if (item.MenuItem != null && item.MenuItem.MenuItemId == menuItemId
                    && item.Status == OrderItemStatus.Ordered)
                {
                    existing = item;
                    break;
                }
            }

            if (existing != null)
            {
                existing.Qty++;
                _repository.UpdateOrderItem(existing);
            }
            else
            {
                MenuItem menuItem = _menuItemRepository.GetMenuItemById(menuItemId);
                OrderItem newItem = new OrderItem
                {
                    Order = order,
                    MenuItem = menuItem,
                    Name = menuItem.Name,
                    Price = menuItem.Price,
                    Vat = menuItem.Vat,
                    ItemType = menuItem.Category,
                    Qty = 1,
                    CreatedAt = DateTime.Now
                };
                _repository.AddOrderItem(newItem);
            }

            _tableRepository.UpdateTableStatus(tableId, TableStatus.Occupied);
        }

        public void DecreaseItemForTable(int orderItemId, int tableId)
        {
            OrderItem item = _repository.GetOrderItemById(orderItemId);
            if (item != null)
            {
                if (item.Qty > 1)
                {
                    item.Qty--;
                    _repository.UpdateOrderItem(item);
                }
                else
                {
                    _repository.DeleteOrderItem(orderItemId);
                }
            }

            RefreshTableStatus(tableId);
        }

        public void RemoveItemFromTable(int orderItemId, int tableId)
        {
            _repository.DeleteOrderItem(orderItemId);
            RefreshTableStatus(tableId);
        }

        public void ServeOrderItem(int orderItemId)
        {
            _repository.UpdateOrderItemStatus(orderItemId, OrderItemStatus.Served);
        }

        private void RefreshTableStatus(int tableId)
        {
            List<OrderItem> items = _repository.GetOrderItemsByTableId(tableId);
            TableStatus status = items.Count > 0 ? TableStatus.Occupied : TableStatus.Free;
            _tableRepository.UpdateTableStatus(tableId, status);
        }
    }
}
