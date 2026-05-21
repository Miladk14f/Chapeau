using Chapeau.Models;

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
    }
}
