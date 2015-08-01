namespace Internet.Chess.Server.Fics
{
    using System;

    public class GameInfo
    {
        /// <summary>
        /// Gets or sets the game id.
        /// </summary>
        public int GameId { get; set; }

        /// <summary>
        /// Gets or sets the white player.
        /// </summary>
        public Player WhitePlayer { get; set; }

        /// <summary>
        /// Gets or sets the black player.
        /// </summary>
        public Player BlackPlayer { get; set; }

        /// <summary>
        /// Gets or sets the game type.
        /// </summary>
        public GameType Type { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this game is private.
        /// </summary>
        /// <value>
        ///   <c>true</c> if private; otherwise, <c>false</c>.
        /// </value>
        public bool Private { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this game is rated.
        /// </summary>
        /// <value>
        ///   <c>true</c> if rated; otherwise, <c>false</c>.
        /// </value>
        public bool Rated { get; set; }

        /// <summary>
        /// Gets or sets the time on clock when game started.
        /// </summary>
        public TimeSpan ClockStart { get; set; }

        /// <summary>
        /// Gets or sets the time increment after each move.
        /// </summary>
        public TimeSpan TimeIncrement { get; set; }

        /// <summary>
        /// Gets or sets the partner's game id.
        /// </summary>
        public int PartnersGameId { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether white player uses time seal.
        /// </summary>
        public bool WhiteUsesTimeSeal { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether black player uses time seal.
        /// </summary>
        public bool BlackUsesTimeSeal { get; set; }
    }
}
