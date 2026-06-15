using Chapeau.Models;
namespace Chapeau.Repositories.BillRepository
{
    public interface IBillRepository
    {
        List<Bill> GetAllBills();
        Bill GetBillById(int id);
        Bill GetBillByOrderId(int orderId);
        int AddBill(Bill bill);
        void UpdateBill(Bill bill);
        void DeleteBill(int id);
    }
}
