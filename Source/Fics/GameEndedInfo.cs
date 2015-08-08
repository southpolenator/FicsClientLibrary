using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Internet.Chess.Server.Fics
{
    public class GameEndedInfo
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

        public string Message { get; set; }

        public double WhitePlayerPoints { get; set; }

        public double BlackPlayerPoints { get; set; }
    }
}
