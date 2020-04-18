using System;
using System.Linq;
using FluentAssertions;
using MongoDB.Driver;
using Moshna.Bot.ChatStatistics;
using NUnit.Framework;
using Telegram.Bot.Types;

namespace Moshna.Bot.Tests
{
    [TestFixture]
    public class StatisticWrapperTests
    {
        private readonly IStatisticWrapper statisticWrapper;
        private readonly IMongoCollection<MessageStatistic> messageStatisticCollection;

        public StatisticWrapperTests()
        {
            var mongoClient = new MongoClient("mongodb://localhost:27017/test_bot_bd");
            var db = mongoClient.GetDatabase("test_bot_bd");
            this.messageStatisticCollection = db.GetCollection<MessageStatistic>("MessageStatisticCollection");
            this.statisticWrapper = new StatisticWrapper(this.messageStatisticCollection);
        }

        [SetUp]
        public void RunBeforeTest()
        {
            messageStatisticCollection.DeleteMany(FilterDefinition<MessageStatistic>.Empty);
        }

        [Test]
        public void Save_Message()
        {
            var message = new Message
            {
                Chat = new Chat
                {
                    Id = 523
                },
                Text = "some text",
                Date = DateTime.Now,
                From = new User
                {
                    Username = "user name"
                }
            };

            statisticWrapper.ProcessMessageAsync(message).Wait();

            var savedEntity = messageStatisticCollection.Find(x => x.UserName == message.From.Username).Single();
            savedEntity.UserName.Should().Be(message.From.Username);
            savedEntity.ChatId.Should().Be(message.Chat.Id);
            savedEntity.MessagesCount.Should().Be(1);
            savedEntity.CharsCount.Should().Be(message.Text.Length);
            savedEntity.Day.Should().Be(new DateTime(message.Date.Year, message.Date.Month, message.Date.Day));
        }

        [Test]
        public void Can_Get_Total_Channel_Messages()
        {
            const long chatId = 2424;
            const string userName = "some_user";
            var firstMessage = new MessageStatistic
            {
                ChatId = chatId,
                CharsCount = 10,
                MessagesCount = 23,
                UserName = userName,
                Day = DateTime.UtcNow
            };
            var secondMessage = new MessageStatistic
            {
                ChatId = chatId,
                CharsCount = 33,
                MessagesCount = 66,
                UserName = userName,
                Day = DateTime.UtcNow
            };
            messageStatisticCollection.InsertMany(new[] { firstMessage, secondMessage });

            var result = statisticWrapper.GetTotalOrderedStatisticsAsync(chatId).Result.Single();

            result.ChatId.Should().Be(chatId);
            result.UserName.Should().Be(userName);
            result.CharsCount.Should().Be(firstMessage.CharsCount + secondMessage.CharsCount);
            result.MessagesCount.Should().Be(firstMessage.MessagesCount + secondMessage.MessagesCount);
        }
    }
}