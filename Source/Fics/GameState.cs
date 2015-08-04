namespace Internet.Chess.Server.Fics
{
    using System;
    using System.Collections.Generic;

    public class GameState
    {
        /// <summary>
        /// Gets or sets the game id.
        /// </summary>
        public int GameId { get; set; }

        /// <summary>
        /// Gets or sets the white player's username.
        /// </summary>
        public string WhitePlayerUsername { get; set; }

        /// <summary>
        /// Gets or sets the black player's username.
        /// </summary>
        public string BlackPlayerUsername { get; set; }

        /// <summary>
        /// Gets or sets the remaining time on white player's clock.
        /// </summary>
        public TimeSpan WhiteClock { get; set; }

        /// <summary>
        /// Gets or sets the remaining time on black player's clock.
        /// </summary>
        public TimeSpan BlackClock { get; set; }

        /// <summary>
        /// Gets or sets the time on clock when game started.
        /// </summary>
        public TimeSpan ClockStart { get; set; }

        /// <summary>
        /// Gets or sets the time increment after each move.
        /// </summary>
        public TimeSpan TimeIncrement { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether it is currently white player's move.
        /// </summary>
        public bool WhiteMove { get; set; }

        /// <summary>
        /// Gets or sets the move number.
        /// </summary>
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
        public ChessPieceWithColor[,] Board { get; set; }
        public List<ChessPieceType> WhitePieces { get; set; }
        public List<ChessPieceType> BlackPieces { get; set; }
    }
}
