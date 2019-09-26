using System.Configuration;

namespace Moshna.Bot
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            var token = config.AppSettings.Settings["BotToken"].Value;
            var sentimentService = new SentimentService("data.txt");
            sentimentService.AddToData("Мошна", true);
            var bot = new Bot(sentimentService, token);
            bot.Start();
        }
    }
}