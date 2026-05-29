using Chapeau.Models;
using Chapeau.Models.Enums;
using Chapeau.Services;
using Chapeau.Services.BillService;
using Chapeau.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Chapeau.Controllers
{
    public class ManagerController : Controller
    {
        private readonly IRestaurantTableService _tableService;
        private readonly IMenuItemService _menuItemService;
        private readonly IBillService _billService;
        private readonly IStaffService _staffService;
        private readonly ICommentService _commentService;
        private readonly IOrderService _orderService;
        private readonly IOrderItemService _orderItemService;

        public ManagerController(
            IRestaurantTableService tableService,
            IMenuItemService menuItemService,
            IBillService billService,
            IStaffService staffService,
            ICommentService commentService,
            IOrderService orderService,
            IOrderItemService orderItemService)
        {
            _tableService = tableService;
            _menuItemService = menuItemService;
            _billService = billService;
            _staffService = staffService;
            _commentService = commentService;
            _orderService = orderService;
            _orderItemService = orderItemService;
        }

        public IActionResult Index(string tab = "revenue", string stockFilter = "all")
        {
            List<Bill> bills       = _billService.GetAllBills();
            List<RestaurantTable> tables = _tableService.GetAllTables();
            List<MenuItem> menu    = _menuItemService.GetAllMenuItems();
            List<Staff> staff      = _staffService.GetAllStaff();
            List<Comment> comments = _commentService.GetAllComments();
            List<Order> orders     = _orderService.GetAllOrders();
            List<OrderItem> allItems = _orderItemService.GetAllOrderItems();

            var paidBills   = bills.Where(b => b.Status == EBillStatus.Paid).ToList();
            var unpaidBills = bills.Where(b => b.Status != EBillStatus.Paid).ToList();
            decimal revPaid = paidBills.Sum(b => b.Amount);
            decimal revOpen = unpaidBills.Sum(b => b.Amount);
            decimal tips    = paidBills.Sum(b => b.Tip);

            var occupiedTables = tables.Where(t => t.Status == ETableStatus.Occupied).ToList();
            int coversSeated   = occupiedTables.Sum(t => t.Guests ?? 0);
            int coversTotal    = tables.Sum(t => t.Guests ?? 0);

            var stockItems = menu.Select(m => new StockItemRow
            {
                Name     = m.Name,
                Category = m.Category,
                Price    = m.Price,
                Vat      = m.Vat,
                Quantity = m.InStock ? 1 : 0,
                IsLow    = false,
                IsOut    = !m.InStock
            }).ToList();

            int stockOut = stockItems.Count(s => s.IsOut);

            var categoryRevenues = allItems
                .GroupBy(i => i.ItemType.ToString())
                .Select(g => new CategoryRevenue
                {
                    Category = g.Key,
                    Amount   = g.Sum(i => i.Price * i.Qty)
                })
                .ToList();

            decimal sum9   = allItems.Where(i => i.Vat == 9).Sum(i => i.Price * i.Qty);
            decimal sum21  = allItems.Where(i => i.Vat == 21).Sum(i => i.Price * i.Qty);
            decimal excl9  = sum9  > 0 ? sum9  / 1.09m : 0;
            decimal excl21 = sum21 > 0 ? sum21 / 1.21m : 0;

            var orderItemsByOrder = allItems
                .GroupBy(i => i.Order?.OrderId ?? 0)
                .ToDictionary(g => g.Key, g => g.ToList());

            var staffPerformances = orders
                .Where(o => o.Staff != null)
                .GroupBy(o => o.Staff.StaffId)
                .Select(g =>
                {
                    var member   = staff.FirstOrDefault(s => s.StaffId == g.Key);
                    string name  = member?.Name ?? "Unknown";
                    string initials = name.Length >= 2
                        ? $"{name[0]}{name.LastOrDefault(c => c == ' ' ? false : name.IndexOf(c) > 0 && name[name.IndexOf(c) - 1] == ' ')}"
                        : name[..1].ToUpper();
                    var parts   = name.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                    initials    = parts.Length >= 2
                        ? $"{parts[0][0]}{parts[^1][0]}".ToUpper()
                        : name[..Math.Min(2, name.Length)].ToUpper();

                    var groupItems = g
                        .SelectMany(o => orderItemsByOrder.TryGetValue(o.OrderId, out var it) ? it : new List<OrderItem>())
                        .ToList();

                    return new StaffPerformance
                    {
                        Name    = name,
                        Initials = initials,
                        Items   = groupItems.Sum(i => i.Qty),
                        Revenue = groupItems.Sum(i => i.Price * i.Qty)
                    };
                })
                .Where(s => s.Revenue > 0)
                .ToList();

            var openTables = occupiedTables.Select(t =>
            {
                string waiterName = "";
                if (t.Waiter != null)
                {
                    var waiter = staff.FirstOrDefault(s => s.StaffId == t.Waiter.StaffId);
                    waiterName = waiter?.Name ?? "";
                }

                var tableOrderIds = orders
                    .Where(o => o.Table?.TableId == t.TableId && o.Status != EOrderStatus.Paid)
                    .Select(o => o.OrderId)
                    .ToHashSet();

                var tableItems = allItems
                    .Where(i => tableOrderIds.Contains(i.Order?.OrderId ?? -1))
                    .ToList();

                return new OpenTableRow
                {
                    TableId   = t.TableId,
                    Guests    = t.Guests ?? 0,
                    WaiterName = waiterName,
                    ItemCount = tableItems.Sum(i => i.Qty),
                    Total     = tableItems.Sum(i => i.Price * i.Qty)
                };
            }).ToList();

            var orderDict = orders.ToDictionary(o => o.OrderId);
            var feedbackItems = comments.Select(c =>
            {
                int tableId    = 0;
                string staffName = "";
                if (c.Order != null && orderDict.TryGetValue(c.Order.OrderId, out var order))
                {
                    tableId = order.Table?.TableId ?? 0;
                    if (order.Staff != null)
                    {
                        var waiter = staff.FirstOrDefault(s => s.StaffId == order.Staff.StaffId);
                        staffName  = waiter?.Name ?? "";
                    }
                }
                return new FeedbackRow
                {
                    Type      = c.Type.ToString(),
                    TableId   = tableId,
                    Text      = c.Text,
                    Staff     = staffName,
                    CreatedAt = c.CreatedAt
                };
            }).ToList();

            var vm = new ManagerDashboardViewModel
            {
                ActiveTab   = tab,
                StockFilter = stockFilter,

                RevenuePaid      = revPaid,
                RevenueOpen      = revOpen,
                CoversToday      = coversTotal,
                CoversSeated     = coversSeated,
                TipsReceived     = tips,
                TablesCheckedOut = paidBills.Count,
                StockOutCount    = stockOut,
                StockLowCount    = 0,

                CategoryRevenues  = categoryRevenues,
                StaffPerformances = staffPerformances,

                Vat9ExclBtw  = excl9,
                Vat9Amount   = sum9  - excl9,
                Vat21ExclBtw = excl21,
                Vat21Amount  = sum21 - excl21,

                StockItems = stockItems,

                CommentCount  = comments.Count(c => c.Type == ECommentType.Comment),
                ComplaintCount = comments.Count(c => c.Type == ECommentType.Complaint),
                PraiseCount   = comments.Count(c => c.Type == ECommentType.Praise),
                FeedbackItems = feedbackItems,

                OpenTables = openTables
            };

            return View(vm);
        }

        [HttpGet]
        public IActionResult StaffManagement()
        {
            List<Staff> staff = _staffService.GetAllStaff();
            return View(staff);
        }

        [HttpPost]
        public IActionResult CreateStaff(string name, string role, string pin)
        {
            Staff staff = new Staff
            {
                Name = name,
                Role = Enum.Parse<EStaffRole>(role, ignoreCase: true),
                Pin = pin
            };

            _staffService.AddStaff(staff);
            return RedirectToAction("StaffManagement");
        }

        [HttpGet]
        public IActionResult EditStaff(int id)
        {
            Staff staff = _staffService.GetStaffById(id);

            if (staff == null)
                return NotFound();

            return View(staff);
        }

        [HttpPost]
        public IActionResult UpdateStaff(int id, string name, string role, string pin)
        {
            Staff staff = _staffService.GetStaffById(id);

            if (staff == null)
                return NotFound();

            staff.Name = name;
            staff.Role = Enum.Parse<EStaffRole>(role, ignoreCase: true);

            if (!string.IsNullOrWhiteSpace(pin))
            {
                staff.Pin = pin;
                _staffService.UpdateStaff(staff);
            }
            else
            {
                _staffService.UpdateStaffInfo(staff);
            }
            return RedirectToAction("StaffManagement");
        }

        [HttpPost]
        public IActionResult DeleteStaff(int id)
        {
            _staffService.DeleteStaff(id);
            return RedirectToAction("StaffManagement");
        }
    }
}
