using Lumera.Parser.Extensions;

#pragma warning disable NUnit2005 // Consider using Assert.That(actual, Is.EqualTo(expected)) instead of Assert.AreEqual(expected, actual)

namespace Lumera.Parser.Tests
{
    public class ParseDepositsTest
    {
        [TestCase("00000000001111123456789000000000000000000000000000000000000000000000000000000000", "1111", "1234567890")]
        [TestCase("00000000001911123456789000000000000000000000000000000000000000000000000000000000", "1911", "1234567890")]
        [TestCase("00000000001100123456789000000000000000000000000000000000000000000000000000000000", "1100", "1234567890")]
        public void GivenAValidString_ToDepositStartReturnsValidInstance(string serialized, string clearing, string account)
        {
            var depositStart = serialized.ToDepositStart();
            Assert.AreEqual(clearing, depositStart.AccountNumber.Clearing);
            Assert.AreEqual(account, depositStart.AccountNumber.Account);
        }

        [TestCase("00000000001111 123456789000000000000000000000000000000000000000000000000000000000", "1111", "1234567890")]
        [TestCase("01000000001111 123456789000000000000000000000000000000000000000000000000000000000", "1111", "1234567890")]
        public void GivenAnInvalidString_ToDepositStartThrowsArgumentException(string serialized, string clearing, string account)
        {
            Assert.Throws<ArgumentException>(() => serialized.ToDepositStart());
        }

        [TestCase("30000000000000004000000000000000000000009876543210                              ", 4000.00,  "SEK", "9876543210")]
        [TestCase("30000000000000001000000000000000000000009876543210                              ", 1000.00,  "SEK", "9876543210")]
        [TestCase("30000000000000010300000000000000000000009876543210                              ", 10300.00, "SEK", "9876543210")]
        public void GivenAValidString_ToDepositReturnsValidInstance(string serialized, decimal amount, string currency, string reference)
        {
            var deposit = serialized.ToDeposit();
            Assert.AreEqual(amount, deposit.Amount);
            Assert.AreEqual(currency, deposit.Currency);
            Assert.AreEqual(reference, deposit.Reference);
        }

        [TestCase("99 00000000000123400009 00000000 00000100 000000000000000000000000000000000000000000", 1234000.09, 100)]
        [TestCase("99 00000000000123400019 00000000 00000010 000000000000000000000000000000000000000000", 1234000.19, 10)]
        [TestCase("99 00000000000123400100 00000000 00000001 000000000000000000000000000000000000000000", 1234001.00, 1)]
        public void GivenAValidString_ToDepositEndReturnsValidInstance(string serialized, decimal clearing, int account)
        {
            var depositEnd = serialized.Replace(" ", "").ToDepositEnd();
            Assert.AreEqual(clearing, depositEnd.TotalAmount);
            Assert.AreEqual(account, depositEnd.TotalCount);
        }
    }
}
