using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrawledGamesWebRole.Models
{
    public class Game
    {
        public int Id { get; set; }

        public Player WhitePlayer { get; set; }

        public Player BlackPlayer { get; set; }

        public Game PartnersGame { get; set; }

        public int GameType { get; set; }

        public DateTime GameStarted { get; set; }

        public bool Rated { get; set; }

        public TimeSpan ClockStart { get; set; }

        public TimeSpan TimeIncrement { get; set; }

        public string Result { get; set; }

        public GameMove[] WhitePlayerMoves { get; set; }

        public GameMove[] BlackPlayerMoves { get; set; }
    }

    public class Player
    {
        public string Username { get; set; }

        public int Rating { get; set; }
    }

    public class GameMove
    {
        public string Move { get; set; }

        public TimeSpan MoveTime { get; set; }
    }
}
