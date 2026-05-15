using Chapeau.Models;

namespace Chapeau.Services.BillService
{
    public interface IBillService
    {
        List<Bill> GetAll();
        Bill GetById(int id);
        Bill GetByOrderId(int orderId);
        void Add(Bill bill);
        void Update(Bill bill);
        void Delete(int id);
    }
}