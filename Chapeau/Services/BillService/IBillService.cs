using Chapeau.ViewModels;

namespace Chapeau.Services
{
    public interface IBillService
    {
        BillViewModel GetBill(int tableId);
        PaymentConfirmationViewModel GetConfirmation(int orderId);
        int CreateBill(int orderId, decimal amount, decimal tip, bool isSplit);
        void UpdateBillStatus(int billId);
    }
}
