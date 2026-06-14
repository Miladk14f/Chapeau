using Chapeau.Models;
using Chapeau.Models.Enums;
using Chapeau.Repositories.BillRepository;

namespace Chapeau.Services.BillService
{
    public class BillService : IBillService
    {
        private readonly IBillRepository _billRepository;

        public BillService(IBillRepository billRepository)
        {
            _billRepository = billRepository;
        }

        public List<Bill> GetAllBills()
        {
            return _billRepository.GetAllBills();
        }

        public Bill GetBillById(int id)
        {
            return _billRepository.GetBillById(id);
        }

        public Bill GetBillByOrderId(int orderId)
        {
            return _billRepository.GetBillByOrderId(orderId);
        }

        public int AddBill(Bill bill)
        {
            bill.Status = BillStatus.Unpaid;
            return _billRepository.AddBill(bill);
        }

        public void UpdateBill(Bill bill)
        {
            _billRepository.UpdateBill(bill);
        }

        public void DeleteBill(int id)
        {
            _billRepository.DeleteBill(id);
        }
    }
}
