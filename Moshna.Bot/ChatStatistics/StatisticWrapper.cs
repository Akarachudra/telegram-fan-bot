using System;
using System.Collections.Generic;
using System.Linq;
using Telegram.Bot.Types;

namespace Moshna.Bot.ChatStatistics
{
    public static class StatisticWrapper
    {
        private static readonly IList<UserMessagesStatistic> TodayStatistics = new List<UserMessagesStatistic>();
        private static DateTime today;

        static StatisticWrapper()
        {
            today = GetToday();
        }

        public static void ProcessMessage(Message message)
        {
            var currentDay = GetToday();
            if (currentDay > today)
            {
                TodayStatistics.Clear();
                today = currentDay;
            }

            var userName = message.From.Username;
            var userTodayStatistic = TodayStatistics.FirstOrDefault(x => x.UserName == userName);
            if (userTodayStatistic == null)
            {
                TodayStatistics.Add(
                    new UserMessagesStatistic
                    {
                        UserName = userName,
                        CharsCount = message.Text.Length,
                        MessagesCount = 1
                    });
            }
            else
            {
                userTodayStatistic.CharsCount += message.Text.Length;
                userTodayStatistic.MessagesCount++;
            }
        }

        public static IList<UserMessagesStatistic> GetTodayOrderedStatistics()
        {
            return TodayStatistics.OrderByDescending(x => x.MessagesCount).ToList();
        }

        private static DateTime GetToday()
        {
            var dateTime = DateTime.Now;
            var resultDateTime = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day);
            resultDateTime = DateTime.SpecifyKind(resultDateTime, DateTimeKind.Local);
            return resultDateTime;
        }
    }
}