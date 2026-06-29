using Chapeau.Models;
using Chapeau.Models.Enums;
using Chapeau.Repositories;
using Chapeau.Repositories.BillRepository;
using Chapeau.ViewModels;

namespace Chapeau.Services
{
    public class BillService : IBillService
    {
        private readonly IBillRepository _billRepository;
        private readonly IPaymentRepository _paymentRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IRestaurantTableRepository _tableRepository;
        private readonly IStaffRepository _staffRepository;

        public BillService(
            IBillRepository billRepository,
            IPaymentRepository paymentRepository,
            IOrderRepository orderRepository,
            IRestaurantTableRepository tableRepository,
            IStaffRepository staffRepository)
        {
            _billRepository = billRepository;
            _paymentRepository = paymentRepository;
            _orderRepository = orderRepository;
            _tableRepository = tableRepository;
            _staffRepository = staffRepository;
        }

        public BillViewModel GetBill(int tableId)
        {
            RestaurantTable table = _tableRepository.GetTableById(tableId);
            if (table == null)
                return null;

            Order activeOrder = _orderRepository.GetOrdersByTableId(tableId)
                .Where(o => o.Status != OrderStatus.Paid)
                .OrderByDescending(o => o.CreatedAt)
                .FirstOrDefault();
            if (activeOrder == null)
                return null;

            List<OrderItem> items = _orderRepository.GetOrderItemsByOrderId(activeOrder.OrderId);

            return new BillViewModel
            {
                TableId = tableId,
                OrderId = activeOrder.OrderId,
                Guests = table.Guests ?? 0,
                WaiterName = GetWaiterName(table),
                GeneratedAt = DateTime.Now,
                Items = items
            };
        }

        public PaymentConfirmationViewModel GetConfirmation(int orderId)
        {
            Order order = _orderRepository.GetOrderById(orderId);
            if (order == null)
                return null;

            int tableId = order.Table != null ? order.Table.TableId : 0;
            RestaurantTable table = _tableRepository.GetTableById(tableId);
            List<OrderItem> items = _orderRepository.GetOrderItemsByOrderId(orderId);

            Bill bill = _billRepository.GetAllBills()
                .Where(b => b.Order != null && b.Order.OrderId == orderId)
                .OrderByDescending(b => b.BillId)
                .FirstOrDefault();

            List<Payment> payments = bill != null ? _paymentRepository.GetPaymentsByBillId(bill.BillId) : new List<Payment>();
            decimal tip = bill?.Tip ?? 0;
            DateTime? paidAt = payments.Count > 0 ? payments.Max(p => p.PaidAt) : null;

            return new PaymentConfirmationViewModel
            {
                TableId = tableId,
                OrderId = orderId,
                Guests = table?.Guests ?? 0,
                WaiterName = GetWaiterName(table),
                PaidAt = paidAt ?? DateTime.Now,
                Items = items,
                Tip = tip,
                Payments = payments,
                IsSplit = payments.Count > 1
            };
        }

        public int CreateBill(int orderId, decimal amount, decimal tip, bool isSplit)
        {
            Bill bill = new Bill
            {
                Tip = tip,
                SplitedMethod = isSplit ? SplitMethod.Equal : SplitMethod.None,
                Amount = amount,
                Status = BillStatus.Unpaid,
                Order = new Order { OrderId = orderId }
            };
            return _billRepository.AddBill(bill);
        }

        public void UpdateBillStatus(int billId)
        {
            Bill bill = _billRepository.GetBillById(billId);
            if (bill == null)
                return;

            List<Payment> payments = _paymentRepository.GetPaymentsByBillId(billId);

            decimal totalPaid = 0;
            foreach (Payment payment in payments)
                totalPaid += payment.Amount;

            if (totalPaid >= bill.Amount)
                bill.Status = BillStatus.Paid;
            else if (totalPaid > 0)
                bill.Status = BillStatus.Partial;
            else
                bill.Status = BillStatus.Unpaid;

            _billRepository.UpdateBill(bill);
        }

        private string GetWaiterName(RestaurantTable table)
        {
            if (table?.Waiter == null)
                return "";
            Staff waiter = _staffRepository.GetAllStaff()
                .FirstOrDefault(s => s.StaffId == table.Waiter.StaffId);
            return waiter?.Name ?? "";
        }
    }
}
