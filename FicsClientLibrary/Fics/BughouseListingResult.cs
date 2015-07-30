namespace Internet.Chess.Server.Fics
{
    using System.Collections.Generic;

    public class BughouseListingResult
    {
        public IList<Game2x2> Games { get; set; }
        public IList<Partnership> Partnerships { get; set; }
        public IList<Player> Players { get; set; }
    }
}
