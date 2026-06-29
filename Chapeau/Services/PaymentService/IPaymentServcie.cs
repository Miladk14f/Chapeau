using Chapeau.ViewModels;

namespace Chapeau.Services
{
    public interface IPaymentService
    {
        void ProcessPayment(int tableId, int orderId, List<PersonPaymentInput> persons);
        SplitData StartSplitBill(int orderId, List<PersonPaymentInput> persons);
        void AddSplitPersonPayment(int tableId, int billId, decimal amount, string paymentMethod, string feedbackType, string feedbackText);
        void CompleteOrder(int orderId);
        void CloseTable(int tableId, int orderId);
    }
}
