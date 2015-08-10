using Internet.Chess.Server.Fics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameCrawler
{
    class GameMove
    {
        public string Move { get; set; }
        public TimeSpan Time { get; set; }
    }

    class ObservingGame
    {
        public ObservingGame PartnersGame { get; set; }
        public Game Game { get; set; }
        public List<GameMove> WhiteMovesList { get; set; }
        public List<GameMove> BlackMovesList { get; set; }
    }

    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                CrawlGames();
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.ToString());
            }

            Console.ReadLine();
        }

        private static async void CrawlGames()
        {
            FicsClient client = new FicsClient();
            Dictionary<int, ObservingGame> observingGames = new Dictionary<int, ObservingGame>();

            await client.LoginGuest();
            client.ServerVariables.ShowPromptTime = false;
            client.ServerVariables.Seek = false;
            client.ServerVariables.MoveBell = false;
            client.ServerVariables.Style = 12;
            client.ServerVariables.ShowProvisionalRatings = true;
            client.ServerVariables.Interface = "TestAppUniversal";
            client.ServerInterfaceVariables.PreciseTimes = true;
            client.ServerInterfaceVariables.DetailedGameInfo = true;
            client.ServerInterfaceVariables.PreMove = true;
            client.ServerInterfaceVariables.SmartMove = false;

            client.UnknownMessageReceived += (message) =>
            {
                Console.WriteLine(message);
            };

            client.GameStateChange += (state) =>
            {
                if (state.LastMove != null)
                {
                    var game = observingGames[state.GameId];
                    var move = new GameMove()
                    {
                        Move = state.LastMove,
                        Time = state.LastMoveTime,
                    };

                    if (state.WhiteMove)
                    {
                        game.BlackMovesList.Add(move);
                    }
                    else
                    {
                        game.WhiteMovesList.Add(move);
                    }
                }
            };

            client.GameEnded += (game) =>
            {
                // TODO: Save game
            };

            client.GameStoppedObserving += (gameId) =>
            {
                observingGames.Remove(gameId);
            };

            while (true)
            {
                var bughouse = await client.ListBughouse(BughouseListingOptions.Games);
                var games = bughouse.Games;

                foreach (var game in games)
                {
                    if (!observingGames.ContainsKey(game.First.Id))
                    {
                        var observingGame = new ObservingGame();
                        observingGames.Add(game.First.Id, observingGame);
                        var result = await client.StartObservingGame(game.First);

                        // TODO: Collect moves
                        // await client.Send("moves {0}", game.First.Id);
                        if (result.GameInfo.PartnersGameId != game.Second.Id)
                        {
                            await client.StopObservingGame(game.First);
                            observingGames.Remove(game.First.Id);
                            continue;
                        }
                    }

                    // TODO: Observe partners game
                }
            }
        }
    }
}
