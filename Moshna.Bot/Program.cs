using System.Configuration;
using MongoDB.Driver;
using Moshna.Bot.ChatStatistics;
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

            var mongoClient = new MongoClient("mongodb://localhost:27017/bot");
            var db = mongoClient.GetDatabase("bot");
            var messageStatisticCollection = db.GetCollection<MessageStatistic>("MessageStatisticCollection");
            HostFactory.Run(
                x =>
                {
                    x.Service<Bot>(
                        s =>
                        {
                            s.ConstructUsing(name => new Bot(sentimentService, new StatisticWrapper(messageStatisticCollection), token));
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