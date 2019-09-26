using System.Threading;
using Telegram.Bot;

namespace Moshna.Bot
{
    public class Bot : IBot
    {
        private readonly ISentimentService sentimentService;
        private readonly TelegramBotClient botClient;

        public Bot(ISentimentService sentimentService, string token)
        {
            this.sentimentService = sentimentService;
            this.botClient = new TelegramBotClient(token);
            this.botClient.OnMessage += this.BotOnMessage;
        }

        public void Start()
        {
            this.botClient.StartReceiving();
            Thread.Sleep(int.MaxValue);
        }

        private void BotOnMessage(object sender, Telegram.Bot.Args.MessageEventArgs e)
        {
            if (string.IsNullOrEmpty(e.Message.Text))
            {
                return;
            }

            var text = e.Message.Text.ToLower();
            if (text == "!мошна")
            {
            }
            else
            {
                var isMoshna = this.sentimentService.IsMoshna(text);
                if (isMoshna)
                {
                    this.botClient.SendTextMessageAsync(e.Message.Chat, $"{e.Message.AuthorSignature}, МОШНА!!!").Wait();
                }
            }
        }
    }
}