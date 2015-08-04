namespace Internet.Chess.Server.Fics
{
    using System;

    [Flags]
    public enum PlayersListingOptions
    {
        /// <summary>
        /// List only open players.
        /// </summary>
        [ServerVariableName("o")]
        OpenPlayers = 1,

        /// <summary>
        /// List only players for rated matches.
        /// </summary>
        [ServerVariableName("r")]
        RatedMatches = 2,

        /// <summary>
        /// List only free players (not playing a game).
        /// </summary>
        [ServerVariableName("f")]
        FreePlayers = 4,

        /// <summary>
        /// List only available players (open & free).
        /// </summary>
        [ServerVariableName("a")]
        AvailablePlayers = 8,

        /// <summary>
        /// List only registered players.
        /// </summary>
        [ServerVariableName("R")]
        RegisteredPlayers = 16,

        /// <summary>
        /// List only unregistered players.
        /// </summary>
        [ServerVariableName("U")]
        UnregisteredPlayers = 32,

        /// <summary>
        /// Show and order by Standard rating.
        /// </summary>
        [ServerVariableName("s")]
        StandardRating = 64,

        /// <summary>
        /// Show and order by Blitz rating.
        /// </summary>
        [ServerVariableName("b")]
        BlitzRating = 128,

        /// <summary>
        /// Show and order by Wild rating.
        /// </summary>
        [ServerVariableName("w")]
        WildRating = 256,

        /// <summary>
        /// Show and order by Crazyhouse rating.
        /// </summary>
        [ServerVariableName("z")]
        CrazyhouseRating = 512,

        /// <summary>
        /// Show and order by Lightning rating.
        /// </summary>
        [ServerVariableName("L")]
        LightningRating = 1024,

        /// <summary>
        /// Show and order by Suicide Chess rating.
        /// </summary>
        [ServerVariableName("S")]
        SuicideChessRating = 2048,

        /// <summary>
        /// Show and order by Bughouse rating.
        /// </summary>
        [ServerVariableName("B")]
        BughouseRating = 4096,

        /// <summary>
        /// Lists players alphabetically.
        /// </summary>
        [ServerVariableName("A")]
        ListAlphabetically = 8192,

        // Unused parameters
        // l: Same as above but without rating/game info.
        // t: Terse display.
        // v: Verbose display.
        // n: Win-loss record.
    }
}
