using System.Text;

namespace Moshna.Bot.Civilization
{
    public class CityResult
    {
        public int CitizensCount { get; set; }

        public int AcademyCount { get; set; }

        public int ScienceSpecialistsCount { get; set; }

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
            var fromSpecialist = 3;
            if (this.HasRationalism)
            {
                fromSpecialist = 5;
            }

            var baseScience = this.CitizensCount + this.AcademyCount * 6 + this.ScienceSpecialistsCount * fromSpecialist;
            if (this.HasPalace)
            {
                baseScience += 4;
            }

            if (this.HasNationalColledge)
            {
                baseScience += 3;
            }

            if (this.HasLibrary)
            {
                baseScience += this.CitizensCount / 2;
            }

            if (this.HasSchool)
            {
                baseScience += this.CitizensCount / 2 + 3;
            }

            totalScience += baseScience;
            if (this.HasNationalColledge)
            {
                totalScience += 0.5 * baseScience;
            }

            if (this.HasUniversity)
            {
                totalScience += 0.33 * baseScience;
                if (this.HasUniversityBoost)
                {
                    totalScience += 0.17 * baseScience;
                }
            }

            if (this.HasRationalism)
            {
                totalScience += 0.1 * baseScience;
            }

            if (this.HasLaboratory)
            {
                totalScience += 0.5 * baseScience;
            }

            return totalScience;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine($"Жителей: {this.CitizensCount}");
            sb.AppendLine($"Академий: {this.AcademyCount}");
            sb.AppendLine($"Научных специалистов: {this.ScienceSpecialistsCount}");
            sb.AppendLine($"Дворец: {ConvertToString(this.HasPalace)}");
            sb.AppendLine($"Библиотека: {ConvertToString(this.HasLibrary)}");
            sb.AppendLine($"Национальный колледж: {ConvertToString(this.HasNationalColledge)}");
            sb.AppendLine($"Университет: {ConvertToString(this.HasUniversity)}");
            sb.AppendLine($"Школа: {ConvertToString(this.HasSchool)}");
            sb.AppendLine($"Лаборатория: {ConvertToString(this.HasLaboratory)}");
            return sb.ToString();
        }

        private static string ConvertToString(bool boolValue)
        {
            return boolValue ? "Да" : "Нет";
        }
    }
}