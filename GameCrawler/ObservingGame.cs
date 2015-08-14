namespace GameCrawler
{
    using Internet.Chess.Server.Fics;
    using System;
    using System.Collections.Generic;

    class ObservingGame
    {
        public ObservingGame PartnersGame { get; set; }
        public Game Game { get; set; }
        public GameEndedInfo Result { get; set; }
        public List<ChessMove> WhiteMovesList { get; set; }
        public List<ChessMove> BlackMovesList { get; set; }
        public DateTime GameStarted { get; set; }
        public bool Finished { get; set; }
    }
}
