using Chapeau.Models;
using Chapeau.Models.Enums;
using Chapeau.Repositories;
using Chapeau.Repositories.BillRepository;

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

        public List<Payment> GetAllPayments()
        {
            return _paymentRepository.GetAllPayments();
        }

        public List<Payment> GetPaymentsByBillId(int billId)
        {
            return _paymentRepository.GetPaymentsByBillId(billId);
        }

        public Payment GetPaymentById(int id)
        {
            return _paymentRepository.GetPaymentById(id);
        }

        public void AddPayment(Payment payment)
        {
            payment.Status = BillStatus.Paid;
            payment.PaidAt = DateTime.Now;

            _paymentRepository.AddPayment(payment);

            UpdateBillStatus(payment.Bill?.BillId ?? 0);
        }

        public void UpdatePayment(Payment payment)
        {
            _paymentRepository.UpdatePayment(payment);

            UpdateBillStatus(payment.Bill?.BillId ?? 0);
        }

        public void DeletePayment(int id)
        {
            Payment payment = _paymentRepository.GetPaymentById(id);

            if (payment != null)
            {
                int billId = payment.Bill?.BillId ?? 0;

                _paymentRepository.DeletePayment(id);

                UpdateBillStatus(billId);
            }
        }

        private void UpdateBillStatus(int billId)
        {
            Bill bill = _billRepository.GetBillById(billId);

            if (bill == null)
            {
                return;
            }

            List<Payment> payments = _paymentRepository.GetPaymentsByBillId(billId);

            decimal totalPaid = payments.Sum(payment => payment.Amount);

            if (totalPaid >= bill.Amount)
            {
                bill.Status = BillStatus.Paid;
            }
            else if (totalPaid > 0)
            {
                bill.Status = BillStatus.Partial;
            }
            else
            {
                bill.Status = BillStatus.Unpaid;
            }

            _billRepository.UpdateBill(bill);
        }
    }
}
