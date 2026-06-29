using Chapeau.Models;
using Chapeau.ViewModels;

namespace Chapeau.Services
{
    public interface IPaymentService
    {
        BillViewModel GetBill(int tableId);
        void ProcessPayment(int tableId, int orderId, List<PersonPaymentInput> persons);
        SplitData StartSplitBill(int orderId, List<PersonPaymentInput> persons);
        void AddSplitPersonPayment(int billId, decimal amount, string paymentMethod);
        void CompleteOrder(int orderId);
        PaymentConfirmationViewModel GetConfirmation(int orderId);
        void CloseTable(int tableId, int orderId);
    }
}
