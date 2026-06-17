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
        private readonly IOrderItemRepository _orderItemRepository;
        private readonly IStaffRepository _staffRepository;
        private readonly IMenuItemService _menuItemService;
        private readonly IRestaurantTableRepository _tableRepository;

        public OrderService(IOrderRepository repository, IOrderItemRepository orderItemRepository, IStaffRepository staffRepository, IMenuItemService menuItemService, IRestaurantTableRepository tableRepository)
        {
            _repository = repository;
            _orderItemRepository = orderItemRepository;
            _staffRepository = staffRepository;
            _menuItemService = menuItemService;
            _tableRepository = tableRepository;
        }

        public OrderViewModel GetOrderPage(int tableId)
        {
            RestaurantTable table = _tableRepository.GetTableById(tableId);
            List<MenuItem> menu = _menuItemService.GetAllMenuItems();
            Staff staff = _staffRepository.GetStaffById(2);

            Order current = _repository.GetActiveOrderByTableId(tableId);
            if (current == null)
            {
                int newOrderId = AddOrder(new Order
                {
                    Table = table,
                    Staff = staff,
                    Status = OrderStatus.InProgress,
                    CreatedAt = DateTime.Now
                });
                current = _repository.GetOrderById(newOrderId);
            }

            List<OrderItem> orderItems = _orderItemRepository.GetOrderItemsByTableId(tableId);
            return new OrderViewModel(menu, orderItems, table, staff);
        }

        public List<Order> GetAllOrders()
        {
            return _repository.GetAllOrders();
        }

        public List<Order> GetOrdersByTableId(int tableId)
        {
            return _repository.GetOrdersByTableId(tableId);
        }

        public List<Order> GetOrdersByStaffId(int staffId)
        {
            return _repository.GetOrdersByStaffId(staffId);
        }

        public Order GetOrderById(int orderId)
        {
            return _repository.GetOrderById(orderId);
        }

        public int AddOrder(Order order)
        {
            order.CreatedAt = DateTime.Now;
            order.Status = OrderStatus.Pending;
            return _repository.AddOrder(order);
        }

        public void UpdateOrder(Order order)
        {
            _repository.UpdateOrder(order);
        }

        public void UpdateOrderStatus(int orderId, OrderStatus status)
        {
            _repository.UpdateOrderStatus(orderId, status);
        }

        public void DeleteOrder(int orderId)
        {
            _repository.DeleteOrder(orderId);
        }

        public Order GetActiveOrderByTableId(int tableId)
        {
            return _repository.GetActiveOrderByTableId(tableId);
        }

        public List<PreparationCard> GetPreparationCards(ItemType[] types, int warningMinutes, int urgentMinutes)
        {
            List<Staff> staff = _staffRepository.GetAllStaff();
            List<Order> allOrders = _repository.GetAllOrders();
            List<OrderItem> allItems = _orderItemRepository.GetAllOrderItems();

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
    }
}
