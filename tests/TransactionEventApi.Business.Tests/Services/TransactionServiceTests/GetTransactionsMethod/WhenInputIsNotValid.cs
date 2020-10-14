using NUnit.Framework;

namespace TransactionEventApi.Business.Tests.Services.TransactionServiceTests.GetTransactionsMethod
{
    [TestFixture]
    public class WhenInputIsNotValid : TransactionServiceTestBase
    {
        [OneTimeSetUp]
        public void Setup()
        {
            base.SharedSetup();
        }

        [Test]
        public void Throws_With_Invalid_Path()
        {
            Assert.That(() => ClassInTest.GetTransactionsAsync(null), ThrowsArgumentNullException("request"));
        }
    }
}
