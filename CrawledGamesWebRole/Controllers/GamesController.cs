using System;
using System.Linq;
using System.Web.Http;
using CrawledGamesWebRole.Models;
using System.Data.SqlClient;

namespace CrawledGamesWebRole.Controllers
{
    [RoutePrefix("api/Games")]
    public class GamesController : ApiController
    {
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

        [Route("Game")]
        public Game GetGame(int id)
        {
            DB.Model db = DB.Model.SqlAzure("<server>", "<db>", "<username>", "<password>");

            var games = db.Games.SqlQuery("SELECT * FROM Game WHERE Id = @id", new SqlParameter("@id", id)).ToListAsync().Result;

            if (games.Count > 0)
            {
                var dbGame = games[0];
                var dbWhiteMoves = dbGame.GameMoves.Where(m => m.WhiteMove.Value).ToArray();
                var dbBlackMoves = dbGame.GameMoves.Where(m => !m.WhiteMove.Value).ToArray();
                var whiteMoves = ConvertMoves(dbWhiteMoves);
                var blackMoves = ConvertMoves(dbBlackMoves);

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
                    PartnerGameId = dbGame.PartnerGameId,
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
    }
}
