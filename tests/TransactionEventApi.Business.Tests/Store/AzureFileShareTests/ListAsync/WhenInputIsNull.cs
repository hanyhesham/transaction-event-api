using System;
using NUnit.Framework;

namespace TransactionEventApi.Business.Tests.Store.AzureFileShareTests.ListAsync
{
    [TestFixture]
    public class WhenInputIsNull : AzureFileShareTestBase
    {
        [OneTimeSetUp]
        public void Setup()
        {
            SharedSetup();
        }
        
        [Test]
        public void Exception_Is_Thrown()
        {
            Assert.That(() => ClassInTest.ListAsync(null),
                Throws.ArgumentNullException.With.Property(nameof(ArgumentNullException.ParamName)).EqualTo("pathFilter"));
        }
    }
}