namespace Internet.Chess.Server.Fics
{
    using System;

    public class Game
    {
        public int Id { get; set; }
        public Player WhitePlayer { get; set; }
        public Player BlackPlayer { get; set; }
        public GameType Type { get; set; }
        public bool Private { get; set; }
        public bool Rated { get; set; }
        public bool Examined { get; set; }
        public bool InSetup { get; set; }
        public TimeSpan WhiteClock { get; set; }
        public TimeSpan BlackClock { get; set; }
        public TimeSpan ClockStart { get; set; }
        public TimeSpan TimeIncrement { get; set; }
        public bool WhiteMove { get; set; }
        public int Move { get; set; }
        public int WhiteStrength { get; set; }
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
