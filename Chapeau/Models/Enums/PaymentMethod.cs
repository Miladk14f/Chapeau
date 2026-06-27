namespace Chapeau.Models.Enums
{
    public enum PaymentMethod
    {
        Cash,
        CreditCard,
        DebitCard,
        // legacy values kept so existing DB rows still parse
        Pin,
        Visa,
        Amex
    }
}
