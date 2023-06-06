namespace Lumera.Parser.Dto
{
    public record Deposit(decimal Amount, string Currency, string Reference, string Reserved = "");
}
