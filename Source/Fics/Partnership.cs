namespace Internet.Chess.Server.Fics
{
    public class Partnership
    {
        /// <summary>
        /// First player
        /// </summary>
        public Player Player1 { get; set; }

        /// <summary>
        /// Second player
        /// </summary>
        public Player Player2 { get; set; }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>A string that represents the current object.</returns>
        public override string ToString()
        {
            return Player1.ToString() + " / " + Player2.ToString();
        }
    }
}
