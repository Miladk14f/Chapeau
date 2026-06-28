using Chapeau.Models;
using Chapeau.Models.Enums;
using Chapeau.Repositories;
using Chapeau.Repositories.BillRepository;
using Chapeau.ViewModels;

namespace Chapeau.Services
{
    public class ManagerService : IManagerService
    {
        private readonly IBillRepository _billRepository;
        private readonly IRestaurantTableRepository _tableRepository;
        private readonly IMenuItemRepository _menuItemRepository;
        private readonly IStaffRepository _staffRepository;
        private readonly ICommentRepository _commentRepository;
        private readonly IOrderRepository _orderRepository;

        public ManagerService(
            IBillRepository billRepository,
            IRestaurantTableRepository tableRepository,
            IMenuItemRepository menuItemRepository,
            IStaffRepository staffRepository,
            ICommentRepository commentRepository,
            IOrderRepository orderRepository)
        {
            _billRepository = billRepository;
            _tableRepository = tableRepository;
            _menuItemRepository = menuItemRepository;
            _staffRepository = staffRepository;
            _commentRepository = commentRepository;
            _orderRepository = orderRepository;
        }

        public ManagerDashboardViewModel GetDashboard(string tab, string stockFilter)
        {
            List<Bill> bills = _billRepository.GetAllBills();
            List<RestaurantTable> tables = _tableRepository.GetAllTables();
            List<MenuItem> menu = _menuItemRepository.GetAllMenuItems();
            List<Staff> staff = _staffRepository.GetAllStaff();
            List<Comment> comments = _commentRepository.GetAllComments();
            List<Order> orders = _orderRepository.GetAllOrders();
            List<OrderItem> allItems = _orderRepository.GetAllOrderItems();

            decimal revPaid = 0;
            decimal revOpen = 0;
            decimal tips = 0;
            int paidBillCount = 0;
            foreach (Bill bill in bills)
            {
                if (bill.Status == BillStatus.Paid)
                {
                    revPaid += bill.Amount;
                    tips += bill.Tip;
                    paidBillCount++;
                }
                else
                {
                    revOpen += bill.Amount;
                }
            }

            int coversSeated = 0;
            int coversTotal = 0;
            List<RestaurantTable> occupiedTables = new List<RestaurantTable>();
            foreach (RestaurantTable table in tables)
            {
                coversTotal += table.Guests ?? 0;
                if (table.Status == TableStatus.Occupied)
                {
                    occupiedTables.Add(table);
                    coversSeated += table.Guests ?? 0;
                }
            }

            List<StockItemRow> stockItems = new List<StockItemRow>();
            int stockOut = 0;
            foreach (MenuItem item in menu)
            {
                bool isOut = !item.InStock;
                stockItems.Add(new StockItemRow
                {
                    Name = item.Name,
                    Category = item.Category.ToString(),
                    Price = item.Price,
                    Vat = item.Vat,
                    Quantity = item.InStock ? 1 : 0,
                    IsLow = false,
                    IsOut = isOut
                });
                if (isOut)
                    stockOut++;
            }

            Dictionary<string, decimal> revenueByCategory = new Dictionary<string, decimal>();
            Dictionary<int, List<OrderItem>> itemsByOrder = new Dictionary<int, List<OrderItem>>();
            decimal sum9 = 0;
            decimal sum21 = 0;
            foreach (OrderItem item in allItems)
            {
                decimal lineTotal = item.Price * item.Qty;

                string category = item.ItemType.ToString();
                if (!revenueByCategory.ContainsKey(category))
                    revenueByCategory[category] = 0;
                revenueByCategory[category] += lineTotal;

                if (item.Vat == 9)
                    sum9 += lineTotal;
                else if (item.Vat == 21)
                    sum21 += lineTotal;

                int orderId = item.Order != null ? item.Order.OrderId : 0;
                if (!itemsByOrder.ContainsKey(orderId))
                    itemsByOrder[orderId] = new List<OrderItem>();
                itemsByOrder[orderId].Add(item);
            }

            List<CategoryRevenue> categoryRevenues = new List<CategoryRevenue>();
            foreach (KeyValuePair<string, decimal> pair in revenueByCategory)
                categoryRevenues.Add(new CategoryRevenue { Category = pair.Key, Amount = pair.Value });

            decimal excl9 = sum9 > 0 ? sum9 / 1.09m : 0;
            decimal excl21 = sum21 > 0 ? sum21 / 1.21m : 0;

            Dictionary<int, StaffPerformance> performanceByStaff = new Dictionary<int, StaffPerformance>();
            foreach (Order order in orders)
            {
                if (order.Staff == null)
                    continue;

                int staffId = order.Staff.StaffId;
                if (!performanceByStaff.ContainsKey(staffId))
                {
                    string name = "Unknown";
                    foreach (Staff s in staff)
                    {
                        if (s.StaffId == staffId)
                        {
                            name = s.Name;
                            break;
                        }
                    }
                    performanceByStaff[staffId] = new StaffPerformance
                    {
                        Name = name,
                        Initials = BuildInitials(name)
                    };
                }

                if (itemsByOrder.ContainsKey(order.OrderId))
                {
                    foreach (OrderItem item in itemsByOrder[order.OrderId])
                    {
                        performanceByStaff[staffId].Items += item.Qty;
                        performanceByStaff[staffId].Revenue += item.Price * item.Qty;
                    }
                }
            }

            List<StaffPerformance> staffPerformances = new List<StaffPerformance>();
            foreach (KeyValuePair<int, StaffPerformance> pair in performanceByStaff)
            {
                if (pair.Value.Revenue > 0)
                    staffPerformances.Add(pair.Value);
            }

            List<OpenTableRow> openTables = new List<OpenTableRow>();
            foreach (RestaurantTable table in occupiedTables)
            {
                string waiterName = "";
                if (table.Waiter != null)
                {
                    foreach (Staff s in staff)
                    {
                        if (s.StaffId == table.Waiter.StaffId)
                        {
                            waiterName = s.Name;
                            break;
                        }
                    }
                }

                int itemCount = 0;
                decimal tableTotal = 0;
                foreach (Order order in orders)
                {
                    bool sameTable = order.Table != null && order.Table.TableId == table.TableId;
                    if (!sameTable || order.Status == OrderStatus.Paid)
                        continue;
                    if (!itemsByOrder.ContainsKey(order.OrderId))
                        continue;

                    foreach (OrderItem item in itemsByOrder[order.OrderId])
                    {
                        itemCount += item.Qty;
                        tableTotal += item.Price * item.Qty;
                    }
                }

                openTables.Add(new OpenTableRow
                {
                    TableId = table.TableId,
                    Guests = table.Guests ?? 0,
                    WaiterName = waiterName,
                    ItemCount = itemCount,
                    Total = tableTotal
                });
            }

            Dictionary<int, Order> orderById = new Dictionary<int, Order>();
            foreach (Order order in orders)
                orderById[order.OrderId] = order;

            List<FeedbackRow> feedbackItems = new List<FeedbackRow>();
            int commentCount = 0;
            int complaintCount = 0;
            int praiseCount = 0;
            foreach (Comment comment in comments)
            {
                int tableId = 0;
                string staffName = "";
                if (comment.Order != null && orderById.ContainsKey(comment.Order.OrderId))
                {
                    Order order = orderById[comment.Order.OrderId];
                    tableId = order.Table != null ? order.Table.TableId : 0;
                    if (order.Staff != null)
                    {
                        foreach (Staff s in staff)
                        {
                            if (s.StaffId == order.Staff.StaffId)
                            {
                                staffName = s.Name;
                                break;
                            }
                        }
                    }
                }

                feedbackItems.Add(new FeedbackRow
                {
                    Type = comment.Type.ToString(),
                    TableId = tableId,
                    Text = comment.Text,
                    Staff = staffName,
                    CreatedAt = comment.CreatedAt
                });

                if (comment.Type == CommentType.Comment) commentCount++;
                else if (comment.Type == CommentType.Complaint) complaintCount++;
                else if (comment.Type == CommentType.Praise) praiseCount++;
            }

            return new ManagerDashboardViewModel
            {
                ActiveTab = tab,
                StockFilter = stockFilter,

                RevenuePaid = revPaid,
                RevenueOpen = revOpen,
                CoversToday = coversTotal,
                CoversSeated = coversSeated,
                TipsReceived = tips,
                TablesCheckedOut = paidBillCount,
                StockOutCount = stockOut,
                StockLowCount = 0,

                CategoryRevenues = categoryRevenues,
                StaffPerformances = staffPerformances,

                Vat9ExclBtw = excl9,
                Vat9Amount = sum9 - excl9,
                Vat21ExclBtw = excl21,
                Vat21Amount = sum21 - excl21,

                StockItems = stockItems,

                CommentCount = commentCount,
                ComplaintCount = complaintCount,
                PraiseCount = praiseCount,
                FeedbackItems = feedbackItems,

                OpenTables = openTables
            };
        }

        private string BuildInitials(string name)
        {
            string[] parts = name.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length >= 2)
                return $"{parts[0][0]}{parts[^1][0]}".ToUpper();
            if (name.Length >= 2)
                return name.Substring(0, 2).ToUpper();
            if (name.Length == 1)
                return name.ToUpper();
            return "?";
        }
    }
}
