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
        public GameEndedInfo Result { get; set; }
        public List<GameMove> WhiteMovesList { get; set; }
        public List<GameMove> BlackMovesList { get; set; }
    }

    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                CrawlGames(GamesListingOptions.Bughouse | GamesListingOptions.Crazyhouse);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.ToString());
            }

            Console.ReadLine();
        }

        private static async void CrawlGames(GamesListingOptions gameListingOptions)
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
                ObservingGame game;

                lock (observingGames)
                {
                    if (!observingGames.TryGetValue(state.GameId, out game))
                        return;
                }

                if (state.LastMove != null)
                {
                    var move = new GameMove()
                    {
                        Move = state.LastMove,
                        Time = state.LastMoveTime,
                    };

                    List<GameMove> movesList = state.WhiteMove ? game.BlackMovesList : game.WhiteMovesList;

                    lock (movesList)
                    {
                        while (movesList.Count < state.Move)
                        {
                            movesList.Add(null);
                        }

                        movesList[state.Move - 1] = move;
                    }
                }
            };

            client.GameEnded += (result) =>
            {
                lock (observingGames)
                {
                    ObservingGame game;

                    if (observingGames.TryGetValue(result.GameId, out game))
                    {
                        game.Result = result;
                        Console.WriteLine("Game ended {0}", game.Game);
                        observingGames.Remove(result.GameId);

                        // TODO: Save game
                    }
                }
            };

            //client.GameStoppedObserving += (gameId) =>
            //{
            //    lock (observingGames)
            //    {
            //        ObservingGame game;

            //        if (observingGames.TryGetValue(gameId, out game))
            //        {
            //            Console.WriteLine("Removing game {0}", game.Game);
            //            observingGames.Remove(gameId);
            //        }
            //    }
            //};

            while (true)
            {
                var games = await client.ListGames(gameListingOptions);

                Console.WriteLine(games.Count);
                foreach (var game in games)
                {
                    bool containsKey;

                    lock (observingGames)
                    {
                        containsKey = observingGames.ContainsKey(game.Id);
                    }

                    if (!containsKey)
                    {
                        // TODO: If we started observing different game, stop observing it

                        // Add game to the list
                        var observingGame = new ObservingGame();
                        observingGame.BlackMovesList = new List<GameMove>();
                        observingGame.WhiteMovesList = new List<GameMove>();
                        observingGame.Game = game;
                        lock (observingGames)
                        {
                            observingGames.Add(game.Id, observingGame);
                        }

                        // Start observing game
                        var result = await client.StartObservingGame(game);
                        Console.WriteLine("Starting game {0}", game);

                        // TODO: Collect and update moves list (with lock)
                        await client.Send("moves {0}", game.Id);

                        // Connect partners game
                        if (result.GameInfo.PartnersGameId > 0)
                        {
                            ObservingGame partnersGame;

                            lock (observingGames)
                            {
                                if (observingGames.TryGetValue(result.GameInfo.PartnersGameId, out partnersGame))
                                {
                                    partnersGame.PartnersGame = observingGame;
                                    observingGame.PartnersGame = partnersGame;
                                }
                            }
                        }
                    }
                }

                await Task.Delay(TimeSpan.FromSeconds(10));
            }
        }
    }
}
