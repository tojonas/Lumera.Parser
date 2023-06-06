
using Lumera.Parser.Dto;
using Lumera.Parser.Extensions;

#pragma warning disable NUnit2005 // Consider using Assert.That(actual, Is.EqualTo(expected)) instead of Assert.AreEqual(expected, actual)

namespace Lumera.Parser.Tests
{
    public class StringExtensionParseTests
    {
        [TestCase("12345 67890123456", 1, 1, 1)]
        [TestCase("12345 67890123456", 1, 2, 12)]
        public void CanParseNumber(string pattern, int start, int end, int result)
        {
            Assert.AreEqual(result, pattern.Parse<int>(start, end));
        }

        [TestCase("12345 67890123456", 1, 1, 1)]
        [TestCase("12345 67890123456", 1, 2, 12)]
        [TestCase("12345 67890123456", 7, 17, 67890123456)]
        public void CanParseLong(string pattern, int start, int end, long result)
        {
            Assert.AreEqual(result, pattern.Parse<long>(start, end));
        }

        [TestCase("1,234567890123456", 1, 1, 1.0)]
        [TestCase("12,34567890123456", 1, 2, 12.0)]
        [TestCase("123,4567890123456", 1, 4, 123.0)]
        [TestCase("123,4567890123456", 1, 5, 123.4)]
        public void CanParseDecimal(string pattern, int start, int end, decimal result)
        {
            Assert.AreEqual(result, pattern.Parse<decimal>(start, end));
        }

        [TestCase("1,234567890123456SEK", 18, 20, "SEK")]
        [TestCase("12,34567890123456NOK", 18, 20, "NOK")]
        [TestCase("123,4567890123456USD", 18, 20, "USD")]
        [TestCase("123,4567890123456EUR", 18, 20, "EUR")]
        public void CanParseString(string pattern, int start, int end, string result)
        {
            Assert.AreEqual(result, pattern.Parse<string>(start, end));
        }

        [TestCase("123,4567890123456EUR ", 18, 21, "EUR")]
        [TestCase("123,4567890123456 EUR", 18, 21, " EUR")]
        public void CanParsePaddedString(string pattern, int start, int end, string result)
        {
            Assert.AreEqual(result, pattern.Parse<string>(start, end));
        }

        [TestCase("20230501", 1, 8, "2023-05-01")]
        public void CanParseDateOnly(string pattern, int start, int end, string result)
        {
            Assert.AreEqual(DateOnly.Parse(result), pattern.Parse<DateOnly>(start, end));
        }

        [TestCase("1122 1122334455", "1122", "1122334455")]
        public void CanParseAccountNumber(string pattern, string clearing, string account)
        {
            var accountNumber = AccountNumber.Parse(pattern);
            Assert.AreEqual(clearing, accountNumber.Clearing);
            Assert.AreEqual(account, accountNumber.Account);
        }

        [TestCase("11221122334455")]
        [TestCase("1122  1122334455")]
        [TestCase("1122 11ab334455")]
        [TestCase("11ab 1122334455")]
        public void ThrowWhenInvalidAccountNumber(string pattern)
        {
            Assert.Throws<ArgumentException>(() => AccountNumber.Parse(pattern));
        }
    }
}
