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

        public ManagerController(
            IRestaurantTableService tableService,
            IMenuItemService menuItemService,
            IBillService billService,
            IStaffService staffService,
            ICommentService commentService)
        {
            _tableService = tableService;
            _menuItemService = menuItemService;
            _billService = billService;
            _staffService = staffService;
            _commentService = commentService;
        }

        public IActionResult Index(string tab = "revenue", string stockFilter = "all")
        {
            List<Bill> bills = _billService.GetAllBills();
            List<RestaurantTable> tables = _tableService.GetAllTables();
            List<MenuItem> menu = _menuItemService.GetAllMenuItems();
            List<Staff> staff = _staffService.GetAllStaff();
            List<Comment> comments = _commentService.GetAllComments();

            var paidBills = bills.Where(b => b.Status == EBillStatus.Paid).ToList();
            var unpaidBills = bills.Where(b => b.Status != EBillStatus.Paid).ToList();
            decimal revPaid = paidBills.Sum(b => b.Amount);
            decimal revOpen = unpaidBills.Sum(b => b.Amount);
            decimal tips = paidBills.Sum(b => b.Tip);

            var occupiedTables = tables.Where(t => t.Status == ETableStatus.Occupied).ToList();
            int coversSeated = occupiedTables.Sum(t => t.Guests ?? 0);
            int coversTotal = tables.Sum(t => t.Guests ?? 0);

            var stockItems = menu.Select(m => new StockItemRow
            {
                Name = m.Name,
                Category = m.Category,
                Price = m.Price,
                Vat = m.Vat,
                Quantity = m.InStock ? 1 : 0,
                IsLow = false,
                IsOut = !m.InStock
            }).ToList();

            int stockOut = stockItems.Count(s => s.IsOut);

            var openTables = occupiedTables.Select(t =>
            {
                string waiterName = "";
                if (t.WaiterId.HasValue)
                {
                    var waiter = staff.FirstOrDefault(s => s.Id == t.WaiterId.Value);
                    waiterName = waiter?.Name ?? "";
                }
                return new OpenTableRow
                {
                    TableId = t.Id,
                    Guests = t.Guests ?? 0,
                    WaiterName = waiterName,
                    ItemCount = 0,
                    Total = 0
                };
            }).ToList();

            var feedbackItems = comments.Select(c => new FeedbackRow
            {
                Type = c.Type.ToString(),
                TableId = 0,
                Text = c.Text,
                Staff = "",
                CreatedAt = c.CreatedAt
            }).ToList();

            var vm = new ManagerDashboardViewModel
            {
                ActiveTab = tab,
                StockFilter = stockFilter,

                RevenuePaid = revPaid,
                RevenueOpen = revOpen,
                CoversToday = coversTotal,
                CoversSeated = coversSeated,
                TipsReceived = tips,
                TablesCheckedOut = paidBills.Count,
                StockOutCount = stockOut,
                StockLowCount = 0,

                CategoryRevenues = new(),
                StaffPerformances = new(),
                Vat9ExclBtw = 0,
                Vat9Amount = 0,
                Vat21ExclBtw = 0,
                Vat21Amount = 0,

                StockItems = stockItems,

                CommentCount = comments.Count(c => c.Type == ECommentType.Comment),
                ComplaintCount = comments.Count(c => c.Type == ECommentType.Complaint),
                PraiseCount = comments.Count(c => c.Type == ECommentType.Praise),
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
