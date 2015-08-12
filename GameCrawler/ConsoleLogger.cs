namespace GameCrawler
{
    using System;

    class ConsoleLogger : ILogger
    {
        public void LogException(Exception exception)
        {
            Console.Error.WriteLine("Exception: {0}", exception);
        }

        public void LogUnknownMessage(string message)
        {
            Console.Error.WriteLine("Uknown message: '{0}'", message);
        }

        public void SaveGame(ObservingGame game)
        {
            // Do nothing
        }
    }
}
