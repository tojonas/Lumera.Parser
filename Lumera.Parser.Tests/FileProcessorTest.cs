using Lumera.Parser.Processors;

namespace Lumera.Parser.Tests
{
    // Naive tests just to show how to use the FileProcessor
    public class FileProcessorTest
    {
        [Test]
        public void GivenUniqueFilePatterns_FileProcessorRegisterHandlers()
        {
            var processor = new FileProcessor();
            processor.Register("_inbetalningstjansten.txt", new DepositProcessor(new PaymentReceiver()));
            processor.Register("_betalningsservice.txt", new PaymentProcessor(new PaymentReceiver()));
        }

        [Test]
        public void GivenDuplicateFilePatterns_FileProcessorThrowsException()
        {
            var processor = new FileProcessor();
            processor.Register("_inbetalningstjansten.txt", new DepositProcessor(new PaymentReceiver()));
            Assert.Throws<ArgumentException>(() => processor.Register("_inbetalningstjansten.txt", new PaymentProcessor(new PaymentReceiver())));
        }

        [TestCase(null)]
        [TestCase("")]
        public void GivenInvalidPath_FileProcessorThrowsArgumentException(string path)
        {
            var processor = new FileProcessor();
            Assert.Throws<ArgumentException>(() => processor.ProcessFile(path));
        }

        [TestCase(@"c:\file\doesn't\exist")]
        public void GivenNonExistingFile_FileProcessorThrowsFileNotFoundException(string path)
        {
            var processor = new FileProcessor();
            Assert.Throws<FileNotFoundException>(() => processor.ProcessFile(path));
        }

        [TestCase(@"..\..\..\Data\Exempelfil_betalningsservice.txt")]
        public void GivenExistingFileAndNoConfiguredParser_FileProcessorIgnoresFile(string path)
        {
            var processor = new FileProcessor();
            Assert.IsFalse(processor.ProcessFile(path));
        }

        // This test fails since the sum is wrong in the start record
        // Sum of payments read 4700,17 does not match start record amount 4711,17
        [TestCase(@"..\..\..\Data\Exempelfil_betalningsservice.txt")]
        public void GivenExistingPaymentFileAndConfiguredParser_FileProcessorReadsTheFile(string path)
        {
            var processor = new FileProcessor();
            processor.Register("_betalningsservice.txt", new PaymentProcessor(new PaymentReceiver()));
            Assert.IsTrue(processor.ProcessFile(path));
        }

        [TestCase(@"..\..\..\Data\Exempelfil_inbetalningstjansten.txt")]
        public void GivenExistingDepositFileAndConfiguredParser_FileProcessorReadsTheFile(string path)
        {
            var processor = new FileProcessor();
            processor.Register("_inbetalningstjansten.txt", new DepositProcessor(new PaymentReceiver()));
            Assert.IsTrue(processor.ProcessFile(path));
        }
        [TestCase(@"..\..\..\Data\Exempelfil_inbetalningstjansten.txt")]
        public void GivenDepositFileMultipleTimes_FileProcessorParsesAll(string path)
        {
            var processor = new FileProcessor();
            processor.Register("_inbetalningstjansten.txt", new DepositProcessor(new PaymentReceiver()));
            Assert.IsTrue(processor.ProcessFile(path));
            Assert.IsTrue(processor.ProcessFile(path));
        }
        // This shows how to configure the file processor to handle multiple file types by registering a parser per file "type"
        [Test]
        public void GivenSameFileMultipleTimes_FileProcessorParsesAll()
        {
            var processor = new FileProcessor();
            processor.Register("_inbetalningstjansten.txt", new DepositProcessor(new PaymentReceiver()));
            processor.Register("_betalningsservice.txt", new PaymentProcessor(new PaymentReceiver()));
            processor.Register("_betalningsservice_fix.txt", new PaymentProcessor(new PaymentReceiver()));

            Assert.IsFalse(processor.ProcessFile(@"..\..\..\Data\Exempelfil_nosupport.txt"));

            Assert.IsTrue(processor.ProcessFile(@"..\..\..\Data\Exempelfil_inbetalningstjansten.txt"));
            Assert.IsTrue(processor.ProcessFile(@"..\..\..\Data\Exempelfil_inbetalningstjansten.txt"));

            // Indata file is invalid: Sum of payments read 4700,17 does not match start record amount 4711,17 
            Assert.IsTrue(processor.ProcessFile(@"..\..\..\Data\Exempelfil_betalningsservice_fix.txt"));
            Assert.IsTrue(processor.ProcessFile(@"..\..\..\Data\Exempelfil_betalningsservice.txt"));
        }

        [TestCase(@"..\..\..\Data\Exempelfil_nosupport.txt")]
        public void GivenNonSupportedFile_FileProcessorProcessFileReturnsFalse(string path)
        {
            var processor = new FileProcessor();
            processor.Register("_inbetalningstjansten.txt", new DepositProcessor(new PaymentReceiver()));
            processor.Register("_betalningsservice.txt", new PaymentProcessor(new PaymentReceiver()));

            Assert.IsFalse(processor.ProcessFile(path));
        }
    }
}

