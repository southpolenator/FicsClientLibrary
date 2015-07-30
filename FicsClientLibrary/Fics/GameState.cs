namespace Internet.Chess.Server.Fics
{
    using System;
    using System.Collections.Generic;

    public class GameState
    {
        public int GameId { get; set; }
        public ChessPieceWithColor[,] Board { get; set; }
        public string WhitePlayerUsername { get; set; }
        public string BlackPlayerUsername { get; set; }
        public TimeSpan ClockStart { get; set; }
        public TimeSpan TimeIncrement { get; set; }
        public TimeSpan WhiteClock { get; set; }
        public TimeSpan BlackClock { get; set; }
        public bool WhiteMove { get; set; }
        public int Move { get; set; }
        public bool BlackAtBottom { get; set; }
        public TimeSpan LastMoveTime { get; set; }
        public string LastMove { get; set; }
        public string LastMoveVerbose { get; set; }
        public int DoublePushPawnColumn { get; set; }
        public bool WhiteCanCastleShort { get; set; }
        public bool WhiteCanCastleLong { get; set; }
        public bool BlackCanCastleShort { get; set; }
        public bool BlackCanCastleLong { get; set; }
        public int MovesSinceLastIrreversibleMove { get; set; }
        public List<ChessPiece> WhitePieces { get; set; }
        public List<ChessPiece> BlackPieces { get; set; }
    }
}
