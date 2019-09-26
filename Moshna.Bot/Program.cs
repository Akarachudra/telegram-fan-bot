using System.Configuration;

namespace Moshna.Bot
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            var token = config.AppSettings.Settings["BotToken"].Value;
            var bot = new Bot(token);
            bot.Start();
        }
    }
}