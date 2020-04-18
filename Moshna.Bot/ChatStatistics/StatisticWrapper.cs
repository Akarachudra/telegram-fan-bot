using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using Telegram.Bot.Types;

namespace Moshna.Bot.ChatStatistics
{
    public class StatisticWrapper : IStatisticWrapper
    {
        private readonly IMongoCollection<MessageStatistic> messageStatisticCollection;

        public StatisticWrapper(IMongoCollection<MessageStatistic> messageStatisticCollection)
        {
            this.messageStatisticCollection = messageStatisticCollection;
        }

        public async Task ProcessMessageAsync(Message message)
        {
            var chatId = message.Chat.Id;
            var userName = message.From.Username;
            var messageDay = DateTimeToLocalDay(message.Date);
            var existsMessage = (await messageStatisticCollection.FindAsync(x => x.Day == messageDay && x.UserName == userName && x.ChatId == chatId))
                .SingleOrDefault();
            if (existsMessage == null)
            {
                await messageStatisticCollection.InsertOneAsync(
                    new MessageStatistic
                    {
                        ChatId = chatId,
                        Day = messageDay,
                        UserName = userName
                    });
            }

            var updateDefinition = new UpdateDefinitionBuilder<MessageStatistic>().Inc(x => x.CharsCount, message.Text.Length).Inc(x => x.MessagesCount, 1);
            await messageStatisticCollection.UpdateOneAsync(x => x.Day == messageDay && x.UserName == userName && x.ChatId == chatId, updateDefinition);
        }

        public async Task<IList<MessageStatistic>> GetTodayOrderedStatisticsAsync(long chatId)
        {
            var today = DateTimeToLocalDay(DateTime.UtcNow);
            var statistics = await (await messageStatisticCollection.FindAsync(x => x.Day == today && x.ChatId == chatId)).ToListAsync();
            return statistics.OrderByDescending(x => x.MessagesCount).ThenBy(x => x.CharsCount).ToList();
        }

        public async Task<IList<MessageStatistic>> GetTotalOrderedStatisticsAsync(long chatId)
        {
            var result = await messageStatisticCollection.Aggregate()
                                   .Match(x => x.ChatId == chatId)
                                   .Group(
                                       y => y.UserName,
                                       g => new
                                       {
                                           UserName = g.Key,
                                           CharsCount = g.Sum(x => x.CharsCount),
                                           MessagesCount = g.Sum(x => x.MessagesCount)
                                       })
                                   .ToListAsync();
            return result.Select(
                             x => new MessageStatistic
                             {
                                 ChatId = chatId,
                                 CharsCount = x.CharsCount,
                                 MessagesCount = x.MessagesCount,
                                 UserName = x.UserName
                             })
                         .ToList();
        }

        private static DateTime DateTimeToLocalDay(DateTime dateTime)
        {
            dateTime = dateTime.ToLocalTime();
            var resultDateTime = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day);
            resultDateTime = DateTime.SpecifyKind(resultDateTime, DateTimeKind.Utc);
            return resultDateTime;
        }
    }
}