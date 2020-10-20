using Glasswall.Administration.K8.TransactionEventApi.Business.Enums;
using Glasswall.Administration.K8.TransactionEventApi.Common.Enums;
using NUnit.Framework;

namespace TransactionEventApi.Business.Tests.Enums
{
    [TestFixture]
    public class RiskAssessmentTests
    {
        [Test]
        [TestCase(NCFSOutcome.Relayed, GwOutcome.Unknown, Risk.AllowedByNCFS)]
        [TestCase(NCFSOutcome.Relayed, GwOutcome.Replace, Risk.AllowedByNCFS)]
        [TestCase(NCFSOutcome.Relayed, GwOutcome.Unmodified, Risk.AllowedByNCFS)]
        [TestCase(NCFSOutcome.Relayed, GwOutcome.Failed, Risk.AllowedByNCFS)]
        [TestCase(NCFSOutcome.Replaced, GwOutcome.Unknown, Risk.Safe)]
        [TestCase(NCFSOutcome.Replaced, GwOutcome.Replace, Risk.Safe)]
        [TestCase(NCFSOutcome.Replaced, GwOutcome.Unmodified, Risk.Safe)]
        [TestCase(NCFSOutcome.Replaced, GwOutcome.Failed, Risk.Safe)]
        [TestCase(NCFSOutcome.Blocked, GwOutcome.Unknown, Risk.BlockedByNCFS)]
        [TestCase(NCFSOutcome.Blocked, GwOutcome.Replace, Risk.BlockedByNCFS)]
        [TestCase(NCFSOutcome.Blocked, GwOutcome.Unmodified, Risk.BlockedByNCFS)]
        [TestCase(NCFSOutcome.Blocked, GwOutcome.Failed, Risk.BlockedByNCFS)]
        [TestCase(NCFSOutcome.Unknown, GwOutcome.Unknown, Risk.Unknown)]
        [TestCase(NCFSOutcome.Unknown, GwOutcome.Replace, Risk.Safe)]
        [TestCase(NCFSOutcome.Unknown, GwOutcome.Unmodified, Risk.AllowedByPolicy)]
        [TestCase(NCFSOutcome.Unknown, GwOutcome.Failed, Risk.BlockedByPolicy)]
        public void Correct_Risk_Is_Identified(NCFSOutcome ncfsOutcome, GwOutcome gwOutcome, Risk expected)
        {
            var actual = RiskAssessment.DetermineRisk(ncfsOutcome, gwOutcome);
            Assert.That(actual, Is.EqualTo(expected));
        }
    }
}
