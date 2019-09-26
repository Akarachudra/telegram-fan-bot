using System.Threading;
using Telegram.Bot;

namespace Moshna.Bot
{
    public class Bot : IBot
    {
        private readonly TelegramBotClient botClient;

        public Bot(string token)
        {
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
            if (e.Message.Text == "Проверка бота")
            {
                this.botClient.SendTextMessageAsync(e.Message.Chat, "Работаю!").Wait();
            }
        }
    }
}