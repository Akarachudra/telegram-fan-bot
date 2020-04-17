﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
            var messageDay = DateTimeToDay(message.Date);
            var existsMessage = (await this.messageStatisticCollection.FindAsync(x => x.Day == messageDay && x.UserName == userName && x.ChatId == chatId))
                .SingleOrDefault();
            if (existsMessage == null)
            {
                await this.messageStatisticCollection.InsertOneAsync(
                    new MessageStatistic
                    {
                        ChatId = chatId,
                        Day = messageDay,
                        UserName = userName
                    });
            }

            var updateDefinition = new UpdateDefinitionBuilder<MessageStatistic>().Inc(x => x.CharsCount, message.Text.Length).Inc(x => x.MessagesCount, 1);
            await this.messageStatisticCollection.UpdateOneAsync(x => x.Day == messageDay && x.UserName == userName && x.ChatId == chatId, updateDefinition);
        }

        public async Task<IList<MessageStatistic>> GetTodayOrderedStatisticsAsync(long chatId)
        {
            var today = DateTimeToDay(DateTime.UtcNow);
            var statistics = await (await this.messageStatisticCollection.FindAsync(x => x.Day == today && x.ChatId == chatId)).ToListAsync();
            return statistics.OrderByDescending(x => x.MessagesCount).ThenBy(x => x.CharsCount).ToList();
        }

        private static DateTime DateTimeToDay(DateTime dateTime)
        {
            var resultDateTime = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day);
            resultDateTime = DateTime.SpecifyKind(resultDateTime, DateTimeKind.Utc);
            return resultDateTime;
        }
    }
}