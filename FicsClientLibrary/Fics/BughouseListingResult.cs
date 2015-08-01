namespace Internet.Chess.Server.Fics
{
    using System.Collections.Generic;

    public class BughouseListingResult
    {
        /// <summary>
        /// Gets or sets the list of games.
        /// </summary>
        public IList<Game2x2> Games { get; set; }

        /// <summary>
        /// Gets or sets the list of partnerships.
        /// </summary>
        public IList<Partnership> Partnerships { get; set; }

        /// <summary>
        /// Gets or sets the list of available partners.
        /// </summary>
        public IList<Player> Players { get; set; }
    }
}
