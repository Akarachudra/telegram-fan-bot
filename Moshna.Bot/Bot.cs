using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Moshna.Bot.ChatStatistics;
using Moshna.Bot.Civilization;
using Telegram.Bot;
using Telegram.Bot.Args;
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
            botClient = new TelegramBotClient(token);
            botClient.OnMessage += BotOnMessage;
            botClient.OnUpdate += BotOnUpdate;
        }

        public void Start()
        {
            botClient.StartReceiving();
        }

        public void Stop()
        {
            botClient.StopReceiving();
        }

        private async void BotOnUpdate(object sender, UpdateEventArgs e)
        {
            // throw new System.NotImplementedException();
        }

        private async void BotOnMessage(object sender, MessageEventArgs e)
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
                    sentimentService.AddToData(reply.Text, true);
                }
            }
            else if (text == "/немошна")
            {
                var reply = message.ReplyToMessage;
                if (reply != null && reply.Text != "/немошна")
                {
                    sentimentService.AddToData(reply.Text, false);
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
                    await botClient.SendTextMessageAsync(message.Chat, result.ToString());
                }
                catch
                {
                    // ignored
                }
            }
            else if (text.StartsWith("/флуд_полная_статистика"))
            {
                var userStatistics = await statisticWrapper.GetTotalOrderedStatisticsAsync(message.Chat.Id);
                await SendStatistics(userStatistics, message);
            }
            else if (text.StartsWith("/флуд"))
            {
                var userStatistics = await statisticWrapper.GetTodayOrderedStatisticsAsync(message.Chat.Id);
                await SendStatistics(userStatistics, message);
            }
            else if (text.StartsWith("/опрос"))
            {
                // var poll = await this.botClient.SendPollAsync(message.Chat.Id, "Srabotaet?", new[] { "Да", "Нет" }, isAnonymous: false);
            }
            else
            {
                await statisticWrapper.ProcessMessageAsync(message);
                // turn off moshna checking
                return;
                var isMoshna = sentimentService.IsMoshna(text);
                if (isMoshna)
                {
                    await botClient.SendTextMessageAsync(message.Chat, $"{message.From.Username}, МОШНА!!!");
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

            await botClient.SendTextMessageAsync(message.Chat, stringBuilder.ToString());
        }
    }
}