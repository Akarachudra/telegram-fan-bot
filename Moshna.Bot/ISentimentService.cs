namespace Moshna.Bot
{
    public interface ISentimentService
    {
        void AddToData(string text, bool isMoshna);

        bool IsMoshna(string text);
    }
}