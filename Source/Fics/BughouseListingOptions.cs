namespace Internet.Chess.Server.Fics
{
    using System;

    [Flags]
    public enum BughouseListingOptions
    {
        /// <summary>
        /// Lists partnerships that are not playing bughouse
        /// </summary>
        [ServerVariableName("p")]
        Partnerships = 1,

        /// <summary>
        /// Lists unpartnered players with bugopen on
        /// </summary>
        [ServerVariableName("u")]
        Players = 2,

        /// <summary>
        /// Lists bughouse games in progress
        /// </summary>
        [ServerVariableName("g")]
        Games = 4,
    }
}
