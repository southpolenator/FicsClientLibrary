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
                var dbGame = CreateGame(game);

                if (game.PartnersGame != null)
                {
                    var dbPartnersGame = CreateGame(game.PartnersGame);

                    dbGame.PartnersGame = dbPartnersGame;
                }

                model.Games.Add(dbGame);
            });
        }

        private DB.Game CreateGame(ObservingGame game)
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
                Result = game.Result.Message,
            };

            for (int i = 0; i < game.WhiteMovesList.Count; i++)
            {
                var move = game.WhiteMovesList[i];

                dbGame.GameMoves.Add(new DB.GameMove()
                {
                    Game = dbGame,
                    Move = move.Move,
                    MoveTime = move.Time,
                    WhiteMove = true,
                    MoveNumber = i+1,
                });
            }

            for (int i = 0; i < game.BlackMovesList.Count; i++)
            {
                var move = game.BlackMovesList[i];

                dbGame.GameMoves.Add(new DB.GameMove()
                {
                    Game = dbGame,
                    Move = move.Move,
                    MoveTime = move.Time,
                    WhiteMove = false,
                    MoveNumber = i + 1,
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
                {
                    model = DB.Model.SqlAzure(serverName, databaseName, username, password);
                    model.Configuration.AutoDetectChangesEnabled = false;
                }

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
