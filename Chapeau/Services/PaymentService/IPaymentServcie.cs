using Chapeau.Models;

namespace Chapeau.Services
{
    public interface IPaymentService
    {
        List<Payment> GetAll();
        List<Payment> GetByBillId(int billId);
        Payment GetById(int id);
        void Add(Payment payment);
        void Update(Payment payment);
        void Delete(int id);
    }
}
