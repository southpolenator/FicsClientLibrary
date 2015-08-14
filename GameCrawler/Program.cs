namespace GameCrawler
{
    using Internet.Chess.Server.Fics;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    class Program
    {
        private static Dictionary<int, ObservingGame> observingGames = new Dictionary<int, ObservingGame>();

        static void Main(string[] args)
        {
            try
            {
                ILogger logger = new ConsoleLogger();

                while (true)
                {
                    Log("Starting new crawler");
                    CrawlGames(logger, GamesListingOptions.Bughouse | GamesListingOptions.Crazyhouse).Wait();
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.ToString());
            }

            Log("Ende");
            Console.ReadLine();
        }

        private static void Log(string format, params object[] parameters)
        {
            Console.WriteLine(@"{0:HH\:mm\:ss.fff} {1}", DateTime.Now, string.Format(format.Replace("\n", "\n             "), parameters));
        }

        private static async Task CrawlGames(ILogger logger, GamesListingOptions gameListingOptions)
        {
            FicsClient client = new FicsClient();

            await client.LoginGuest();
            client.ServerVariables.ShowPromptTime = false;
            client.ServerVariables.Seek = false;
            client.ServerVariables.MoveBell = false;
            client.ServerVariables.Style = 12;
            client.ServerVariables.ShowProvisionalRatings = true;
            client.ServerVariables.Interface = "GameCrawler";
            client.ServerInterfaceVariables.PreciseTimes = true;
            client.ServerInterfaceVariables.DetailedGameInfo = true;
            client.ServerInterfaceVariables.PreMove = true;
            client.ServerInterfaceVariables.SmartMove = false;

            client.UnknownMessageReceived += (message) =>
            {
                logger.LogUnknownMessage(message);
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
                    var move = new ChessMove()
                    {
                        Move = state.LastMove,
                        Time = state.LastMoveTime,
                    };

                    List<ChessMove> movesList = state.WhiteMove ? game.BlackMovesList : game.WhiteMovesList;
                    int moveNumber = !state.WhiteMove ? state.Move : state.Move - 1;

                    lock (movesList)
                    {
                        while (movesList.Count < moveNumber)
                        {
                            movesList.Add(null);
                        }

                        movesList[moveNumber - 1] = move;
                    }
                }
            };

            client.GameEnded += (result) =>
            {
                ObservingGame game;

                lock (observingGames)
                {
                    if (observingGames.TryGetValue(result.GameId, out game))
                    {
                        game.Result = result;
                        Log("Game ended {0}\nWhite moves: {1}\nBlack moves: {2}", game.Game, game.WhiteMovesList.Count, game.BlackMovesList.Count);
                        observingGames.Remove(result.GameId);
                    }
                }

                // Check if game was aborted and save otherwise
                if (game != null && result.WhitePlayerPoints + result.BlackPlayerPoints > 0)
                {
                    bool save = true;

                    if (game.PartnersGame != null)
                    {
                        if (game.Game.WhitePlayer.Username.CompareTo(game.PartnersGame.Game.WhitePlayer.Username) < 0)
                        {
                            lock (game)
                            lock (game.PartnersGame)
                            {
                                game.Finished = true;
                                if (!game.PartnersGame.Finished)
                                    save = false;
                            }
                        }
                        else
                        {
                            lock (game.PartnersGame)
                            lock (game)
                            {
                                game.Finished = true;
                                if (!game.PartnersGame.Finished)
                                    save = false;
                            }
                        }
                    }

                    if (save)
                        logger.SaveGame(game);
                }
            };

            while (true)
            {
                var games = await client.ListGames(gameListingOptions);

                Log("Active games: {0}", games.Count);
                foreach (var game in games)
                {
                    if (game.Examined || game.InSetup)
                        continue;

                    bool containsKey;

                    lock (observingGames)
                    {
                        containsKey = observingGames.ContainsKey(game.Id);
                    }

                    if (!containsKey)
                    {
                        try
                        {
                            // Add game to the list
                            var observingGame = new ObservingGame();
                            observingGame.BlackMovesList = new List<ChessMove>();
                            observingGame.WhiteMovesList = new List<ChessMove>();
                            observingGame.Game = game;
                            lock (observingGames)
                            {
                                observingGames.Add(game.Id, observingGame);
                            }

                            // Start observing game
                            var result = await client.StartObservingGame(game);

                            // If we started observing different game, stop observing it
                            if (!result.GameInfo.WhitePlayer.Username.StartsWith(game.WhitePlayer.Username)
                                || !result.GameInfo.BlackPlayer.Username.StartsWith(game.BlackPlayer.Username))
                            {
                                Log("Canceling game {0}", game);
                                await client.StopObservingGame(game);
                            }


                            Log("Starting game {0}", game);

                            // Collect and update moves list
                            var moveList = await client.GetMoveList(game);

                            observingGame.GameStarted = moveList.GameStarted;
                            lock (observingGame.WhiteMovesList)
                            {
                                while (observingGame.WhiteMovesList.Count < moveList.WhiteMoves.Count)
                                    observingGame.WhiteMovesList.Add(null);
                                for (int i = 0; i < moveList.WhiteMoves.Count; i++)
                                    observingGame.WhiteMovesList[i] = moveList.WhiteMoves[i];
                            }

                            lock (observingGame.BlackMovesList)
                            {
                                while (observingGame.BlackMovesList.Count < moveList.BlackMoves.Count)
                                    observingGame.BlackMovesList.Add(null);
                                for (int i = 0; i < moveList.BlackMoves.Count; i++)
                                    observingGame.BlackMovesList[i] = moveList.BlackMoves[i];
                            }

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
                        catch (Exception ex)
                        {
                            logger.LogException(ex);
                            lock (observingGames)
                            {
                                observingGames.Remove(game.Id);
                            }

                            try
                            {
                                await client.StopObservingGame(game.Id);
                            }
                            catch (Exception)
                            {
                                // We want to swallow this exception
                            }
                        }
                    }
                }

                await Task.Delay(TimeSpan.FromSeconds(10));
            }
        }
    }
}
