using Chapeau.Models;
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

        public List<Bill> GetAll()
        {
            return _billRepository.GetAll();
        }

        public Bill GetById(int id)
        {
            return _billRepository.GetById(id);
        }

        public Bill GetByOrderId(int orderId)
        {
            return _billRepository.GetByOrderId(orderId);
        }

        public void Add(Bill bill)
        {
            bill.Status = "unpaid";

            _billRepository.Add(bill);
        }

        public void Update(Bill bill)
        {
            _billRepository.Update(bill);
        }

        public void Delete(int id)
        {
            _billRepository.Delete(id);
        }
    }
}
