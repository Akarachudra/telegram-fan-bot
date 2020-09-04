using System.Text;

namespace Moshna.Bot.Civilization
{
    public class CityResult
    {
        public int CitizensCount { get; set; }

        public int AcademyCount { get; set; }

        public int ScienceSpecialistsCount { get; set; }

        public int SpecialistsCount { get; set; }

        public bool HasPalace { get; set; }

        public bool HasLibrary { get; set; }

        public bool HasNationalColledge { get; set; }

        public bool HasUniversity { get; set; }

        public bool HasSchool { get; set; }

        public bool HasLaboratory { get; set; }

        public bool HasRationalism { get; set; }

        public bool HasUniversityBoost { get; set; }

        public double CalculateScience()
        {
            var totalScience = 0.0;
            var fromScienceSpecialist = 3;
            var fromSpecialist = 0;
            if (HasRationalism)
            {
                fromScienceSpecialist = 5;
                fromSpecialist = 2;
            }

            var baseScience = CitizensCount + AcademyCount * 6 + ScienceSpecialistsCount * fromScienceSpecialist + SpecialistsCount * fromSpecialist;
            if (HasPalace)
            {
                baseScience += 4;
            }

            if (HasNationalColledge)
            {
                baseScience += 3;
            }

            if (HasLibrary)
            {
                baseScience += CitizensCount / 2;
            }

            if (HasSchool)
            {
                baseScience += CitizensCount / 2 + 3;
            }

            totalScience += baseScience;
            if (HasNationalColledge)
            {
                totalScience += 0.5 * baseScience;
            }

            if (HasUniversity)
            {
                totalScience += 0.33 * baseScience;
                if (HasUniversityBoost)
                {
                    totalScience += 0.17 * baseScience;
                }
            }

            if (HasRationalism)
            {
                totalScience += 0.1 * baseScience;
            }

            if (HasLaboratory)
            {
                totalScience += 0.5 * baseScience;
            }

            return totalScience;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine($"Жителей: {CitizensCount}");
            sb.AppendLine($"Академий: {AcademyCount}");
            sb.AppendLine($"Научных специалистов: {ScienceSpecialistsCount}");
            sb.AppendLine($"Дворец: {ConvertToString(HasPalace)}");
            sb.AppendLine($"Библиотека: {ConvertToString(HasLibrary)}");
            sb.AppendLine($"Национальный колледж: {ConvertToString(HasNationalColledge)}");
            sb.AppendLine($"Университет: {ConvertToString(HasUniversity)}");
            sb.AppendLine($"Школа: {ConvertToString(HasSchool)}");
            sb.AppendLine($"Лаборатория: {ConvertToString(HasLaboratory)}");
            return sb.ToString();
        }

        private static string ConvertToString(bool boolValue)
        {
            return boolValue ? "Да" : "Нет";
        }
    }
}