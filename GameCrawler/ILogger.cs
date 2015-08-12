namespace GameCrawler
{
    using System;

    interface ILogger
    {
        void LogUnknownMessage(string message);
        void LogException(Exception exception);
        void SaveGame(ObservingGame game);
    }
}
