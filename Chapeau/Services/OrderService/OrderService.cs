using Chapeau.Models;
using Chapeau.Models.Enums;
using Chapeau.Repositories;
using Chapeau.Services;
using Chapeau.ViewModels;

namespace Chapeau.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _repository;
        private readonly IStaffRepository _staffRepository;
        private readonly IMenuItemService _menuItemService;
        private readonly IMenuItemRepository _menuItemRepository;
        private readonly IRestaurantTableRepository _tableRepository;

        public OrderService(IOrderRepository repository, IStaffRepository staffRepository, IMenuItemService menuItemService, IMenuItemRepository menuItemRepository, IRestaurantTableRepository tableRepository)
        {
            _repository = repository;
            _staffRepository = staffRepository;
            _menuItemService = menuItemService;
            _menuItemRepository = menuItemRepository;
            _tableRepository = tableRepository;
        }

        public OrderViewModel GetOrderPage(int tableId,int staffId)
        {
            RestaurantTable table = _tableRepository.GetTableById(tableId);
            List<MenuItem> menu = _menuItemService.GetAllMenuItems();

            Staff staff = _staffRepository.GetStaffById(staffId);

            Order current = _repository.GetActiveOrderByTableId(tableId);
            if (current == null)
            {
                current = new Order
                {
                    Table = table,
                    Staff = staff,
                    Status = OrderStatus.InProgress,
                    CreatedAt = DateTime.Now
                };

                AddOrder(current);
            }

            List<OrderItem> orderItems = _repository.GetOrderItemsByTableId(tableId);
            return new OrderViewModel(menu, orderItems, table, staff);
        }

        private int AddOrder(Order order)
        {
            order.CreatedAt = DateTime.Now;
            order.Status = OrderStatus.Pending;
            return _repository.AddOrder(order);
        }

        public List<PreparationCard> GetPreparationCards(ItemType[] types, int warningMinutes, int urgentMinutes)
        {
            List<Staff> staff = _staffRepository.GetAllStaff();
            List<Order> allOrders = _repository.GetAllOrders();
            List<OrderItem> allItems = _repository.GetAllOrderItems();

            Dictionary<int, List<OrderItem>> itemsByOrder = new Dictionary<int, List<OrderItem>>();
            foreach (OrderItem item in allItems)
            {
                bool matchesType = false;
                foreach (ItemType t in types)
                {
                    if (item.ItemType == t) { matchesType = true; break; }
                }
                if (!matchesType)
                    continue;
                if (item.Status != OrderItemStatus.Ordered && item.Status != OrderItemStatus.InPreparation)
                    continue;

                int orderId = item.Order != null ? item.Order.OrderId : 0;
                if (!itemsByOrder.ContainsKey(orderId))
                    itemsByOrder[orderId] = new List<OrderItem>();

                itemsByOrder[orderId].Add(item);
            }

            List<PreparationCard> cards = new List<PreparationCard>();
            foreach (Order order in allOrders)
            {
                if (order.Status != OrderStatus.Pending && order.Status != OrderStatus.InProgress)
                    continue;
                if (!itemsByOrder.ContainsKey(order.OrderId))
                    continue;

                List<OrderItem> items = itemsByOrder[order.OrderId];

                int orderStaffId = order.Staff != null ? order.Staff.StaffId : 0;
                string staffName = "Unknown";
                foreach (Staff s in staff)
                {
                    if (s.StaffId == orderStaffId)
                    {
                        staffName = s.Name;
                        break;
                    }
                }

                DateTime orderedAt = items[0].CreatedAt;
                bool hasOrdered = false;
                List<PreparationItemRow> rows = new List<PreparationItemRow>();
                foreach (OrderItem item in items)
                {
                    if (item.CreatedAt < orderedAt)
                        orderedAt = item.CreatedAt;

                    if (item.Status == OrderItemStatus.Ordered)
                        hasOrdered = true;

                    rows.Add(new PreparationItemRow
                    {
                        OrderItemId = item.OrderItemId,
                        Name = item.Name,
                        Qty = item.Qty,
                        CreatedAt = item.CreatedAt,
                        Note = item.Note,
                        Status = item.Status,
                        Category = item.ItemType
                    });
                }

                cards.Add(new PreparationCard
                {
                    OrderId = order.OrderId,
                    TableId = order.Table != null ? order.Table.TableId : 0,
                    StaffName = staffName,
                    OrderedAt = orderedAt,
                    Items = rows,
                    IsPreparing = !hasOrdered,
                    WarningMinutes = warningMinutes,
                    UrgentMinutes = urgentMinutes
                });
            }

            cards.Sort((a, b) => b.MinutesAgo.CompareTo(a.MinutesAgo));
            return cards;
        }

        public List<HistoryCard> GetOrderHistory(ItemType[] types)
        {
            List<Staff> staff = _staffRepository.GetAllStaff();
            List<Order> allOrders = _repository.GetAllOrders();
            List<OrderItem> allItems = _repository.GetAllOrderItems();

            Dictionary<int, List<OrderItem>> itemsByOrder = new Dictionary<int, List<OrderItem>>();
            foreach (OrderItem item in allItems)
            {
                if (item.Status != OrderItemStatus.Ready && item.Status != OrderItemStatus.Served)
                    continue;

                bool matchesType = false;
                foreach (ItemType t in types)
                    if (item.ItemType == t) { matchesType = true; break; }
                if (!matchesType)
                    continue;

                int orderId = item.Order != null ? item.Order.OrderId : 0;
                if (!itemsByOrder.ContainsKey(orderId))
                    itemsByOrder[orderId] = new List<OrderItem>();
                itemsByOrder[orderId].Add(item);
            }

            List<HistoryCard> cards = new List<HistoryCard>();
            foreach (Order order in allOrders)
            {
                if (!itemsByOrder.ContainsKey(order.OrderId))
                    continue;

                int orderStaffId = order.Staff != null ? order.Staff.StaffId : 0;
                string staffName = "Unknown";
                foreach (Staff s in staff)
                    if (s.StaffId == orderStaffId) { staffName = s.Name; break; }

                List<HistoryItemRow> rows = new List<HistoryItemRow>();
                DateTime orderedAt = DateTime.Now;
                foreach (OrderItem item in itemsByOrder[order.OrderId])
                {
                    if (item.CreatedAt < orderedAt) orderedAt = item.CreatedAt;
                    rows.Add(new HistoryItemRow
                    {
                        Name = item.Name,
                        Qty = item.Qty,
                        Status = item.Status,
                        Category = item.ItemType,
                        CreatedAt = item.CreatedAt
                    });
                }

                cards.Add(new HistoryCard
                {
                    OrderId = order.OrderId,
                    TableId = order.Table != null ? order.Table.TableId : 0,
                    StaffName = staffName,
                    OrderedAt = orderedAt,
                    Items = rows
                });
            }

            cards.Sort((a, b) => b.OrderedAt.CompareTo(a.OrderedAt));
            return cards;
        }

        public void UpdateOrderItemStatus(int id, OrderItemStatus status)
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
            Order order = _repository.GetActiveOrderByTableId(tableId);
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
