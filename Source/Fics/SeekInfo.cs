namespace Internet.Chess.Server.Fics
{
    using System;

    public class SeekInfo
    {
        /// <summary>
        /// Gets or sets the seek id.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the player.
        /// </summary>
        public Player Player { get; set; }

        /// <summary>
        /// Gets or sets the game type.
        /// </summary>
        public GameType GameType { get; set; }

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
    }
}
