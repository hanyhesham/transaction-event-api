using NUnit.Framework;

namespace TransactionEventApi.Business.Tests.Store.AzureFileShareTests.ExistsAsync
{
    [TestFixture]
    public class WhenPathIsNotValid : AzureFileShareTestBase
    {
        [OneTimeSetUp]
        public void Setup()
        {
            SharedSetup();
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        public void Exception_Thrown(string path)
        {
            Assert.That(() => ClassInTest.ExistsAsync(path), ThrowsArgumentException("path", "Value must not be null or whitespace"));
        }
    }
}