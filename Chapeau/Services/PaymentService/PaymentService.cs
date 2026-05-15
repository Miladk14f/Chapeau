using Chapeau.Models;
using Chapeau.Repositories;

namespace Chapeau.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly IBillRepository _billRepository;

        public PaymentService(IPaymentRepository paymentRepository, IBillRepository billRepository)
        {
            _paymentRepository = paymentRepository;
            _billRepository = billRepository;
        }

        public List<Payment> GetAll()
        {
            return _paymentRepository.GetAll();
        }

        public List<Payment> GetByBillId(int billId)
        {
            return _paymentRepository.GetByBillId(billId);
        }

        public Payment GetById(int id)
        {
            return _paymentRepository.GetById(id);
        }

        public void Add(Payment payment)
        {
            payment.Status = "Paid";
            payment.PaidAt = DateTime.Now;

            _paymentRepository.Add(payment);

            UpdateBillStatus(payment.BillId);
        }

        public void Update(Payment payment)
        {
            _paymentRepository.Update(payment);

            UpdateBillStatus(payment.BillId);
        }

        public void Delete(int id)
        {
            Payment payment = _paymentRepository.GetById(id);

            if (payment != null)
            {
                int billId = payment.BillId;

                _paymentRepository.Delete(id);

                UpdateBillStatus(billId);
            }
        }

        private void UpdateBillStatus(int billId)
        {
            Bill bill = _billRepository.GetById(billId);

            if (bill == null)
            {
                return;
            }

            List<Payment> payments = _paymentRepository.GetByBillId(billId);

            decimal totalPaid = payments.Sum(payment => payment.Amount);

            if (totalPaid >= bill.Amount)
            {
                bill.Status = "Paid";
            }
            else if (totalPaid > 0)
            {
                bill.Status = "Partially Paid";
            }
            else
            {
                bill.Status = "Unpaid";
            }

            _billRepository.Update(bill);
        }
    }
}