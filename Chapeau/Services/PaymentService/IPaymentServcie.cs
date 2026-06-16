using Chapeau.Models;
using Chapeau.ViewModels;

namespace Chapeau.Services
{
    public interface IPaymentService
    {
        List<Payment> GetAllPayments();
        List<Payment> GetPaymentsByBillId(int billId);
        Payment GetPaymentById(int id);
        void AddPayment(Payment payment);
        void UpdatePayment(Payment payment);
        void DeletePayment(int id);
        BillViewModel GetBill(int tableId);
        void ProcessPayment(int tableId, int orderId, decimal tip, decimal tipOther, string paymentMethod, int splitWays);
    }
}
