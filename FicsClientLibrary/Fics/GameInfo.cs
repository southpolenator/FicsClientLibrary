namespace Internet.Chess.Server.Fics
{
    using System;

    public class GameInfo
    {
        public int GameId { get; set; }
        public Player WhitePlayer { get; set; }
        public Player BlackPlayer { get; set; }
        public bool Private { get; set; }
        public GameType Type { get; set; }
        public bool Rated { get; set; }
        public TimeSpan ClockStart { get; set; }
        public TimeSpan TimeIncrement { get; set; }
        public int PartnersGameId { get; set; }
        public bool WhiteUsesTimeSeal { get; set; }
        public bool BlackUsesTimeSeal { get; set; }
    }
}
