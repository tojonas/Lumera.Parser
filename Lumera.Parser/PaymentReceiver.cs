namespace Lumera.Parser
{
    // This is a dummy implementation of IPaymentReceiver
    // I don't know why there aren't two service endpoints one for Payments and one for deposits since some information is missing.
    // Since we are not calling any external API I'm just going to ignore potential datatype conversions between .NET and Java
    public class PaymentReceiver : IPaymentReceiver
    {
        public void EndPaymentBundle()
        {
        }
        public void Payment(decimal amount, string reference)
        {
        }
        public void StartPaymentBundle(string accountNumber, DateTime paymentDate, string currency)
        {
        }
    }
}
