namespace Internet.Chess.Server.Fics
{
    using System;

    public class Game
    {
        /// <summary>
        /// Gets or sets the game id.
        /// </summary>
        public int Id { get; set; }

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
        /// Gets or sets a value indicating whether this <see cref="Game"/> is private.
        /// </summary>
        /// <value>
        ///   <c>true</c> if private; otherwise, <c>false</c>.
        /// </value>
        public bool Private { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="Game"/> is rated.
        /// </summary>
        /// <value>
        ///   <c>true</c> if rated; otherwise, <c>false</c>.
        /// </value>
        public bool Rated { get; set; }

        public bool Examined { get; set; }

        public bool InSetup { get; set; }

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

        /// <summary>
        /// Gets or sets the white player's strength in pieces.
        /// </summary>
        public int WhiteStrength { get; set; }

        /// <summary>
        /// Gets or sets the black player's strength in pieces.
        /// </summary>
        public int BlackStrength { get; set; }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>A string that represents the current object.</returns>
        public override string ToString()
        {
            if (Examined)
            {
                return string.Format("{0} (Exam. {1} {2})", Id, WhitePlayer, BlackPlayer);
            }

            if (InSetup)
            {
                return string.Format("{0} (Setup {1} {2})", Id, WhitePlayer, BlackPlayer);
            }

            return string.Format("{0} {1} {2}", Id, WhitePlayer, BlackPlayer);
        }
    }
}
