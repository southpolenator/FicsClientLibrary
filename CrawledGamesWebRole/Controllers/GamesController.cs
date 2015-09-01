using System;
using System.Linq;
using System.Web.Http;
using CrawledGamesWebRole.Models;
using System.Data.SqlClient;
using System.Collections.Generic;

namespace CrawledGamesWebRole.Controllers
{
    [RoutePrefix("api/games")]
    public class GamesController : ApiController
    {
        private static Configuration dbConfiguration = new Configuration();

        [Route("game")]
        public Game GetGame(int id)
        {
            var db = GetDb();
            var games = db.Games.Include("GameMoves").Include("PartnersGame").Where(g => g.Id == id);

            return ConvertGame(games.FirstOrDefault(), findPartnersGame: true);
        }

        [Route("top")]
        public Game[] GetTopGames(int count = 100, int skipFirst = 0)
        {
            var db = GetDb();
            var games = db.Games.OrderByDescending(g => g.BlackPlayerRating + g.WhitePlayerRating).Skip(skipFirst).Take(count).ToArray();
            var result = new Game[games.Length];

            for (int i = 0; i < games.Length; i++)
            {
                result[i] = ConvertGame(games[i]);
            }

            return result;
        }

        [Route("total_count")]
        int GetTotalCount()
        {
            var db = GetDb();

            return db.Games.Count();
        }

        [Route("counts")]
        public Dictionary<string, int> GetCounts()
        {
            var db = GetDb();
            Dictionary<string, int> counts = new Dictionary<string, int>();

            counts.Add("total", db.Games.Count());
            counts.Add("crazyhouse", db.Games.Where(g => g.GameType == 7).Count());
            counts.Add("bughouse", db.Games.Where(g => g.PartnersGame != null).Count());
            return counts;
        }

        private static GameMove[] ConvertMoves(DB.GameMove[] dbMoves)
        {
            var moves = new GameMove[dbMoves.Length];

            for (int i = 0; i < dbMoves.Length; i++)
                moves[i] = new GameMove()
                {
                    Move = dbMoves[i].Move,
                    MoveTime = dbMoves[i].MoveTime.Value,
                };
            return moves;
        }

        private static Game ConvertGame(DB.Game dbGame, bool findPartnersGame = false)
        {
            if (dbGame != null)
            {
                GameMove[] whiteMoves = null;
                GameMove[] blackMoves = null;

                if (dbGame.GameMoves != null)
                {
                    var dbWhiteMoves = dbGame.GameMoves.Where(m => m.WhiteMove.Value).OrderBy(m => m.MoveNumber).ToArray();
                    var dbBlackMoves = dbGame.GameMoves.Where(m => !m.WhiteMove.Value).OrderBy(m => m.MoveNumber).ToArray();

                    whiteMoves = ConvertMoves(dbWhiteMoves);
                    blackMoves = ConvertMoves(dbBlackMoves);
                }

                if (findPartnersGame && dbGame.PartnersGame == null)
                {
                    var db = GetDb();

                    dbGame.PartnersGame = db.Games.Include("GameMoves").Where(g => g.PartnerGameId == dbGame.Id).FirstOrDefault();
                }

                return new Game()
                {
                    BlackPlayer = new Player()
                    {
                        Rating = dbGame.BlackPlayerRating.Value,
                        Username = dbGame.BlackPlayer,
                    },
                    ClockStart = dbGame.ClockStart.Value,
                    GameStarted = dbGame.GameStarted.Value,
                    GameType = dbGame.GameType.Value,
                    Id = dbGame.Id,
                    PartnersGame = ConvertGame(dbGame.PartnersGame),
                    Rated = dbGame.Rated.Value,
                    Result = dbGame.Result,
                    TimeIncrement = dbGame.TimeIncrement.Value,
                    WhitePlayer = new Player()
                    {
                        Rating = dbGame.WhitePlayerRating.Value,
                        Username = dbGame.WhitePlayer,
                    },
                    WhitePlayerMoves = whiteMoves,
                    BlackPlayerMoves = blackMoves,
                };
            }

            return null;
        }

        private static DB.Model GetDb()
        {
            var db = DB.Model.SqlAzure(dbConfiguration.DbServer, dbConfiguration.DbName, dbConfiguration.DbUsername, dbConfiguration.DbPassword);
            db.Configuration.LazyLoadingEnabled = false;

            return db;
        }
    }
}
