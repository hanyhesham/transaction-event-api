using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace TransactionEventApi.Business.Tests.Services.TransactionServiceTests.GetDetailMethod
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
        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        public void Throws_With_Invalid_Path(string path)
        {
            Assert.That(() => ClassInTest.GetDetailAsync(path), 
                Throws.ArgumentException.With.Property(nameof(ArgumentException.ParamName))
                    .EqualTo("fileDirectory")
                    .And
                    .With.Property(nameof(ArgumentException.Message))
                    .StartWith("Value must not be null or whitespace"));
        }
    }
}
