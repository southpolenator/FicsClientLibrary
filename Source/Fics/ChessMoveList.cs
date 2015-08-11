namespace Internet.Chess.Server.Fics
{
    using System;
    using System.Collections.Generic;

    public class ChessMoveList
    {
        public int GameId { get; set; }
        public Player WhitePlayer { get; set; }
        public Player BlackPlayer { get; set; }
        public IList<ChessMove> WhiteMoves { get; set; }
        public IList<ChessMove> BlackMoves { get; set; }
        public DateTime GameStarted { get; set; }

        public int CurrentMove
        {
            get
            {
                return !WhiteMove ? WhiteMoves.Count : WhiteMoves.Count + 1;
            }
        }

        /// <summary>
        /// Gets a value indicating whether it is currently white player's move.
        /// </summary>
        public bool WhiteMove
        {
            get
            {
                return WhiteMoves.Count == BlackMoves.Count;
            }
        }
    }
}
