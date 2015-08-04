namespace Internet.Chess.Server.Fics
{
    public enum PlayerProvisionalRating
    {
        /// <summary>
        /// Not queried from server
        /// </summary>
        [ServerVariableName("")]
        Unknown,

        /// <summary>
        /// Players whose rating is not Provisional nor Estimated.
        /// </summary>
        [ServerVariableName(" ")]
        Stable,

        /// <summary>
        /// Players whose rating has never been accompanied by an RD<80.0.
        /// </summary>
        [ServerVariableName("P")]
        Provisional,

        /// <summary>
        /// Players whose displayed rating has been established in the past (RD>80.) and whose RD is now >80.0.
        /// </summary>
        [ServerVariableName("E")]
        Estimated,
    }
}
