using Chapeau.Models;
namespace Chapeau.Repositories.BillRepository
{
    public interface IBillRepository
    {
        List<Bill> GetAllBills();
        Bill GetBillById(int id);
        int AddBill(Bill bill);
        void UpdateBill(Bill bill);
    }
}
