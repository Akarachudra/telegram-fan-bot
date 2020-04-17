using System.Collections.Generic;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace Moshna.Bot.ChatStatistics
{
    public interface IStatisticWrapper
    {
        Task ProcessMessageAsync(Message message);

        Task<IList<MessageStatistic>> GetTodayOrderedStatisticsAsync(long chatId);
    }
}