using Lumera.Parser.Dto;
using System.Globalization;

namespace Lumera.Parser.Extensions
{
    public static class StringExtensions
    {
        public static T Parse<T>(this string value, int start, int end, int decimalPointLocation = -1)
        {
            if (start < 1)
            {
                throw new ArgumentException("Start index must be greater than or equal to one");
            }
            if (end < 1)
            {
                throw new ArgumentException("End index must be greater than or equal to one");
            }
            if (start > end)
            {
                throw new ArgumentException("Start index must be less than end index");
            }
            if (end > value.Length)
            {
                throw new ArgumentException("End index must be less than or equal to the length of the string");
            }
            var adjustedStart = start - 1;
            var part = value.Substring(adjustedStart, end - adjustedStart);
            if (typeof(T) == typeof(string))
            {
                return (T)(object)part.TrimEnd();
            }
            else if (typeof(T) == typeof(AccountNumber))
            {
                return (T)(object)AccountNumber.Parse(part);
            }

            try
            {
                if (decimalPointLocation != -1)
                {
                    part = part.Insert(part.Length - decimalPointLocation, ",");
                }
                return default(T) switch
                {
                    int _ => (T)(object)int.Parse(part),
                    long _ => (T)(object)long.Parse(part),
                    double _ => (T)(object)double.Parse(part),
                    float _ => (T)(object)float.Parse(part),
                    decimal _ => (T)(object)decimal.Parse(part),
                    DateOnly _ => (T)(object)DateOnly.ParseExact(part, "yyyyMMdd", CultureInfo.InvariantCulture),
                    DateTime _ => (T)(object)DateTime.Parse(part),
                    _ => throw new NotImplementedException($"{typeof(T)}"),
                };
            }
            catch (Exception ex)
            {
                throw new FormatException(part, ex);
            }
        }
        public static DepositStart ToDepositStart(this string value)
        {
            if (!value.StartsWith("00"))
            {
                throw new ArgumentException($"Deposit start record [{value}] is not in a valid format");
            }
            return new DepositStart
                            (
                            AccountNumber: new AccountNumber(value.Parse<string>(11, 14), value.Parse<string>(15, 24))
                            );
        }
        public static Deposit ToDeposit(this string value)
        {
            if (!value.StartsWith("30"))
            {
                throw new ArgumentException($"Deposit record [{value}] is not in a valid format");
            }
            return new Deposit
                            (
                            Amount: value.Parse<decimal>(3, 22, 2),
                            Currency: "SEK",
                            Reference: value.Parse<string>(41, 65)
                            );
        }
        public static DepositEnd ToDepositEnd(this string value)
        {
            if (!value.StartsWith("99"))
            {
                throw new ArgumentException($"Deposit end record [{value}] is not in a valid format");
            }
            return new DepositEnd
                            (
                            TotalAmount: value.Parse<decimal>(3, 22, 2),
                            TotalCount: value.Parse<long>(31, 38)
                            );
        }

        public static PaymentStart ToPaymentStart(this string value)
        {
            if (!value.StartsWith("O"))
            {
                throw new ArgumentException($"Payment start record [{value}] is not in a valid format");
            }
            return new PaymentStart
                        (
                        AccountNumber: value.Parse<AccountNumber>(2, 16),
                        TotalAmount: value.Parse<decimal>(17, 30),
                        RecordCount: value.Parse<long>(31, 40),
                        PaymentDate: value.Parse<DateOnly>(41, 48),
                        Currency: value.Parse<string>(49, 51)
                        );
        }

        public static Payment ToPayment(this string value)
        {
            if (!value.StartsWith("B"))
            {
                throw new ArgumentException($"Payment record [{value[0]}] is not in a valid format");
            }
            return new Payment
                            (
                            Amount: value.Parse<decimal>(2, 15),
                            Reference: value.Substring(15)
                            );
        }
    }
}
