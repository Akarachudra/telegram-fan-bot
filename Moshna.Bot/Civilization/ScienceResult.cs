using System;
using System.Collections.Generic;
using System.Text;

namespace Moshna.Bot.Civilization
{
    public class ScienceResult
    {
        public bool IsEnough { get; set; }

        public double TotalScience { get; set; }

        public IList<CityResult> CityResults { get; set; }

        public override string ToString()
        {
            var sb = new StringBuilder();
            var scienceEnoughResult = IsEnough ? "Да" : "Нет";
            sb.AppendLine($"Достижима ли цель по науке: {scienceEnoughResult}");
            sb.AppendLine($"Количество науки с городов: {Math.Round(TotalScience, 2)}");
            var city = 0;
            if (CityResults.Count > 0)
            {
                var cityResult = CityResults[0];
                var rationalismString = cityResult.HasRationalism ? "Да" : "Нет";
                var universityBoostString = cityResult.HasUniversityBoost ? "Да" : "Нет";
                sb.AppendLine($"Рационализм и эффект специалистов: {rationalismString}");
                sb.AppendLine($"Улучшение университетов: {universityBoostString}");
            }

            foreach (var cityResult in CityResults)
            {
                city++;
                sb.AppendLine();
                sb.AppendLine($"Город {city}");
                sb.AppendLine(cityResult.ToString());
            }

            return sb.ToString();
        }
    }
}