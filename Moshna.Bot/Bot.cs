using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Moshna.Bot.ChatStatistics;
using Moshna.Bot.Civilization;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Moshna.Bot
{
    public class Bot : IBot
    {
        private readonly ISentimentService sentimentService;
        private readonly StatisticWrapper statisticWrapper;
        private readonly TelegramBotClient botClient;

        public Bot(ISentimentService sentimentService, StatisticWrapper statisticWrapper, string token)
        {
            this.sentimentService = sentimentService;
            this.statisticWrapper = statisticWrapper;
            this.botClient = new TelegramBotClient(token);
            this.botClient.OnMessage += this.BotOnMessage;
        }

        public void Start()
        {
            this.botClient.StartReceiving();
        }

        public void Stop()
        {
            this.botClient.StopReceiving();
        }

        private async void BotOnMessage(object sender, Telegram.Bot.Args.MessageEventArgs e)
        {
            var message = e.Message;
            if (string.IsNullOrEmpty(message.Text))
            {
                return;
            }

            var text = message.Text.ToLower();
            if (text == "/мошна")
            {
                var reply = message.ReplyToMessage;
                if (reply != null && reply.Text != "/мошна")
                {
                    this.sentimentService.AddToData(reply.Text, true);
                }
            }
            else if (text == "/немошна")
            {
                var reply = message.ReplyToMessage;
                if (reply != null && reply.Text != "/немошна")
                {
                    this.sentimentService.AddToData(reply.Text, false);
                }
            }
            else if (text.StartsWith("/science"))
            {
                var values = text.Split(' ');
                try
                {
                    var science = int.Parse(values[1]);
                    var turn = int.Parse(values[2]);
                    var result = CivCalculator.CalculateCityResultsForScienceAndTurn(science, turn);
                    await this.botClient.SendTextMessageAsync(message.Chat, result.ToString());
                }
                catch
                {
                    // ignored
                }
            }
            else if (text.StartsWith("/флуд"))
            {
                var userStatistics = await this.statisticWrapper.GetTodayOrderedStatisticsAsync(message.Chat.Id);
                await this.SendStatistics(userStatistics, message);
            }
            else if (text.StartsWith("/флуд_полная_статистика"))
            {
                var userStatistics = await this.statisticWrapper.GetTotalOrderedStatisticsAsync(message.Chat.Id);
                await this.SendStatistics(userStatistics, message);
            }
            else
            {
                await this.statisticWrapper.ProcessMessageAsync(message);
                // turn off moshna checking
                return;
                var isMoshna = this.sentimentService.IsMoshna(text);
                if (isMoshna)
                {
                    await this.botClient.SendTextMessageAsync(message.Chat, $"{message.From.Username}, МОШНА!!!");
                }
            }
        }

        private async Task SendStatistics(IEnumerable<MessageStatistic> userStatistics, Message message)
        {
            const string nickname = "User";
            const string messagesCount = "Messages";
            const string charsCount = "Chars";
            var stringBuilder = new StringBuilder();
            stringBuilder.Append($"{nickname.PadRight(20)}{messagesCount.PadRight(20)}{charsCount.PadRight(20)}\n");
            foreach (var userMessagesStatistic in userStatistics)
            {
                stringBuilder.Append(
                    $"{userMessagesStatistic.UserName.PadRight(20)}{userMessagesStatistic.MessagesCount.ToString().PadRight(20)}{userMessagesStatistic.CharsCount.ToString().PadRight(20)}\n");
            }

            await this.botClient.SendTextMessageAsync(message.Chat, stringBuilder.ToString());
        }
    }
}