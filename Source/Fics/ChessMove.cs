namespace Internet.Chess.Server.Fics
{
    using System;

    public class ChessMove
    {
        public string Move { get; set; }
        public TimeSpan Time { get; set; }

        public override string ToString()
        {
            return string.Format(@"{0} ({1:m\:ss\.fff})", Move, Time);
        }
    }
}
