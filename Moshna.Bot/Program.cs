using System.Configuration;
using Topshelf;

namespace Moshna.Bot
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            var token = config.AppSettings.Settings["BotToken"].Value;
            var sentimentService = new SentimentService("data.txt");
            var serviceName = "MoshnaBot";

#if DEBUG
            serviceName = "MoshnaBotDebug";
#endif

            HostFactory.Run(
                x =>
                {
                    x.Service<Bot>(
                        s =>
                        {
                            s.ConstructUsing(name => new Bot(sentimentService, token));
                            s.WhenStarted(tc => tc.Start());
                            s.WhenStopped(tc => tc.Stop());
                        });
                    x.RunAsLocalSystem();

                    x.SetDescription("Rahit group bot");
                    x.SetDisplayName(serviceName);
                    x.SetServiceName(serviceName);
                });
        }
    }
}