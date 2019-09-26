using Moshna.Bot.Civilization;
using NUnit.Framework;

namespace Moshna.Bot.Tests
{
    [TestFixture]
    public class CivCalculatorTests
    {
        [Test]
        public void CalculateForSingleTown()
        {
            var results = CivCalculator.CalculateCityResultsForScienceAndTurn(100, 60);
        }
    }
}