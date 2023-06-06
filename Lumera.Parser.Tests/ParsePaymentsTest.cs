using Lumera.Parser.Extensions;
using System.Globalization;

#pragma warning disable NUnit2005 // Consider using Assert.That(actual, Is.EqualTo(expected)) instead of Assert.AreEqual(expected, actual)

namespace Lumera.Parser.Tests
{
    public class ParsePaymentsTest
    {
        /*
        Öppningspost
        Position	Fält	Datatyp	Kommentar
        1	    Posttyp	Sträng	Fast värde O (stora bokstaven o, ej siffran 0).
        2-16	Kontonummer	Sträng	Anger kontonummer för mottagande konto, inklusive clearingnummer. Inleds med clearingnummer, följt av ett mellanslag, följt av kontonumret. Clearingnummer och kontonummer får bara innehålla siffrorna 0-9 och får inte avdelas med andra tecken.
        17-30	Summa	Decimalt	Anger summan av beloppsfälten i betalningsposterna i filen.
        31-40	Antal	Heltal	Anger antal betalningsposter i filen.
        41-48	Betalningsdatum	Datum	Anger betalningsdatum för betalningarna i filen.
        49-51	Valuta	Sträng	Anger valuta för betalningarna i filen
        */
        [TestCase("O4122 7901021020190000000000,0100000000120220501SEK", "4122", "7901021020", 190000000000.0, 1000000001, "20220501", "SEK")]
        [TestCase("O1234 790102102018000000000,10200000000220220501NOK", "1234", "7901021020", 18000000000.10, 2000000002, "20220501", "NOK")]
        [TestCase("O2345 79010210201700000000,200300000000320210501DEK", "2345", "7901021020", 1700000000.200, 3000000003, "20210501", "DEK")]
        [TestCase("O1122 7901021020160000000,3000400000000420230501USD", "1122", "7901021020", 160000000.3000, 4000000004, "20230501", "USD")]
        public void GivenAValidString_ToPaymentStartReturnsValidInstance(string serialized, string clearing, string account, decimal totalAmount, long recordCount, string paymentDate, string currency)
        {
            var paymentStart = serialized.ToPaymentStart();
            Assert.AreEqual(clearing, paymentStart.AccountNumber.Clearing);
            Assert.AreEqual(account, paymentStart.AccountNumber.Account);
            Assert.AreEqual(totalAmount, paymentStart.TotalAmount);
            Assert.AreEqual(recordCount, paymentStart.RecordCount);
            Assert.AreEqual(DateOnly.ParseExact(paymentDate, "yyyyMMdd", CultureInfo.InvariantCulture), paymentStart.PaymentDate);
            Assert.AreEqual(currency, paymentStart.Currency);
        }

        [TestCase("B00000000000000981111111122222222223333333333abCD", 0, "981111111122222222223333333333abCD")]
        public void GivenAValidString_ToPaymentReturnsValidInstance(string serialized, decimal amount, string account)
        {
            var payment = serialized.ToPayment();
            Assert.AreEqual(amount, payment.Amount);
            Assert.AreEqual(account, payment.Reference);
        }
    }
}
