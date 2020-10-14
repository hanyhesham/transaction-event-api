using System.Threading.Tasks;
using Glasswall.Administration.K8.TransactionEventApi.Common.Enums;
using Glasswall.Administration.K8.TransactionEventApi.Common.Models.V1;
using Moq;
using NUnit.Framework;

namespace TransactionEventApi.Business.Tests.Services.TransactionServiceTests.GetDetailMethod
{
    [TestFixture]
    public class WhenFileDoesNotExist : TransactionServiceTestBase
    {
        private GetDetailResponseV1 _output;
        private const string Input = "some/file/path";


        [OneTimeSetUp]
        public async Task Setup()
        {
            base.SharedSetup();

            _output = await ClassInTest.GetDetailAsync(Input);
        }

        [Test]
        public void Correct_Status_Is_Returned()
        {
            Assert.That(_output, Is.Not.Null);
            Assert.That(_output.Status, Is.EqualTo(DetailStatus.FileNotFound));
        }

        [Test]
        public void AnalysisReport_Is_Not_Returned()
        {
            Assert.That(_output, Is.Not.Null);
            Assert.That(_output.AnalysisReport, Is.Null);
        }

        [Test]
        public void Download_Is_Not_Attempted()
        {
            Share1.Verify(s => s.DownloadAsync(It.IsAny<string>()), Times.Never);
            Share2.Verify(s => s.DownloadAsync(It.IsAny<string>()), Times.Never);
        }

        [Test]
        public void Directory_Is_Checked_For_Existence()
        {
            Share1.Verify(s => s.ExistsAsync(It.Is<string>(x => x == Input)), Times.Once);
            Share2.Verify(s => s.ExistsAsync(It.Is<string>(x => x == Input)), Times.Once);
        }
    }
}