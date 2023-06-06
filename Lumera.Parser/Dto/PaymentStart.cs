namespace Lumera.Parser.Dto
{
    public record PaymentStart(AccountNumber AccountNumber, decimal TotalAmount, long RecordCount, DateOnly PaymentDate, string Currency);
}
