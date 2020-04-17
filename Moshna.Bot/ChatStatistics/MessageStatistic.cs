using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Moshna.Bot.ChatStatistics
{
    public class MessageStatistic
    {
        [BsonId]
        public ObjectId Id { get; set; }

        public long ChatId { get; set; }

        public string UserName { get; set; }

        public DateTime Day { get; set; }

        public long MessagesCount { get; set; }

        public long CharsCount { get; set; }
    }
}