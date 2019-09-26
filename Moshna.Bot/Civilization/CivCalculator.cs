using System.Collections.Generic;

namespace Moshna.Bot.Civilization
{
    public static class CivCalculator
    {
        private const int MaxCitiesCount = 10;
        private const int CitizenPerTurns = 6;
        private const int AcademysPerTurns = 50;
        private const int CityTurnsOffset = 15;
        private const int LibraryTurn = 30;
        private const int NationalColledgeTurn = 65;
        private const int UniversityTurn = 85;
        private const int SchoolTurn = 120;
        private const int LaboratoryTurn = 150;
        private const int RationalismTurn = 120;
        private const int UniversityBoostTurn = 150;

        public static ScienceResult CalculateCityResultsForScienceAndTurn(int science, int turn)
        {
            var result = new ScienceResult { CityResults = new List<CityResult>() };
            var resultScience = 0.0;
            while (resultScience < science && result.CityResults.Count < MaxCitiesCount)
            {
                var currentCity = new CityResult();
                var currentCityOffset = result.CityResults.Count * CityTurnsOffset;
                var currentCitizens = 1 + (turn - currentCityOffset) / CitizenPerTurns;
                if (currentCitizens <= 0)
                {
                    break;
                }

                currentCity.CitizensCount = currentCitizens;

                if (result.CityResults.Count == 0)
                {
                    currentCity.HasPalace = true;
                    currentCity.AcademyCount = turn / AcademysPerTurns;
                    if (turn >= NationalColledgeTurn)
                    {
                        currentCity.HasNationalColledge = true;
                    }
                }

                if (turn >= LibraryTurn + currentCityOffset / 3 && currentCity.CitizensCount > 3)
                {
                    currentCity.HasLibrary = true;
                }

                if (turn >= UniversityTurn + currentCityOffset / 5)
                {
                    currentCity.HasUniversity = true;
                    currentCity.ScienceSpecialistsCount = 2;
                }

                if (turn >= SchoolTurn)
                {
                    currentCity.HasSchool = true;
                    currentCity.ScienceSpecialistsCount = 4;
                }

                if (turn >= LaboratoryTurn)
                {
                    currentCity.HasLaboratory = true;
                    currentCity.ScienceSpecialistsCount = 6;
                }

                if (turn >= RationalismTurn)
                {
                    currentCity.HasRationalism = true;
                }

                if (turn >= UniversityBoostTurn)
                {
                    currentCity.HasUniversityBoost = true;
                }

                result.CityResults.Add(currentCity);
                resultScience += currentCity.CalculateScience();
            }

            result.TotalScience = resultScience;
            if (resultScience >= science)
            {
                result.IsEnough = true;
            }

            return result;
        }
    }
}