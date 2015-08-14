using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameCrawler
{
    class DbLogger : ILogger
    {
        private string serverName;
        private string databaseName;
        private string username;
        private string password;
        private List<DB.Model> models = new List<DB.Model>();

        public DbLogger(string serverName, string databaseName, string username, string password)
        {
            this.serverName = serverName;
            this.databaseName = databaseName;
            this.username = username;
            this.password = password;
        }

        public void LogUnknownMessage(string message)
        {
            Execute((model) =>
            {
                model.Loggers.Add(new DB.Logger()
                {
                    DateTime = DateTime.UtcNow,
                    Message = message,
                });
            });
        }

        public void LogException(Exception exception)
        {
            Execute((model) =>
            {
                model.Loggers.Add(new DB.Logger()
                {
                    DateTime = DateTime.UtcNow,
                    Message = exception.ToString(),
                });
            });
        }

        public void SaveGame(ObservingGame game)
        {
            Execute((model) =>
            {
                if (game.PartnersGame != null)
                {
                    var dbGame1 = AddGame(model, game);
                    var dbGame2 = AddGame(model, game.PartnersGame);

                    dbGame1.PartnersGame = dbGame2;
                    //dbGame2.Game2 = dbGame1;
                }
                else
                {
                    AddGame(model, game);
                }
            });
        }

        private DB.Game AddGame(DB.Model model, ObservingGame game)
        {
            DB.Game dbGame = new DB.Game()
            {
                BlackPlayer = game.Game.BlackPlayer.Username,
                BlackPlayerRating = game.Game.BlackPlayer.Rating,
                ClockStart = game.Game.ClockStart,
                GameStarted = game.GameStarted,
                GameType = (int)game.Game.Type,
                Rated = game.Game.Rated,
                TimeIncrement = game.Game.TimeIncrement,
                WhitePlayer = game.Game.WhitePlayer.Username,
                WhitePlayerRating = game.Game.WhitePlayer.Rating,
            };
            model.Games.Add(dbGame);

            foreach (var move in game.WhiteMovesList)
            {
                model.GameMoves.Add(new DB.GameMove()
                {
                    Game = dbGame,
                    Move = move.Move,
                    MoveTime = move.Time,
                    WhiteMove = true,
                });
            }

            foreach (var move in game.BlackMovesList)
            {
                model.GameMoves.Add(new DB.GameMove()
                {
                    Game = dbGame,
                    Move = move.Move,
                    MoveTime = move.Time,
                    WhiteMove = false,
                });
            }

            return dbGame;
        }

        private void Execute(Action<DB.Model> command)
        {
            for (int i = 0; i < 10; i++)
            {
                try
                {
                    DB.Model model = GetModel();

                    command(model);
                    model.SaveChanges();
                    ReturnModel(model);
                    return;
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine(ex.Message);
                    Task.Delay(TimeSpan.FromSeconds(1)).Wait();
                }
            }

            Console.Error.WriteLine("Command couldn't be executed");
        }

        private DB.Model GetModel()
        {
            lock (models)
            {
                DB.Model model;

                if (models.Count > 0)
                {
                    model = models[models.Count - 1];
                    models.RemoveAt(models.Count - 1);
                }
                else
                    model = DB.Model.SqlAzure(serverName, databaseName, username, password);

                return model;
            }
        }

        private void ReturnModel(DB.Model model)
        {
            lock (models)
            {
                models.Add(model);
            }
        }
    }
}
