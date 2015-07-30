namespace Internet.Chess.Server.Fics
{
    public enum PlayerStatus
    {
        /// <summary>
        /// Ready
        /// </summary>
        [ServerVariableName(" ")]
        Ready,

        /// <summary>
        /// Involved in a game
        /// </summary>
        [ServerVariableName("^")]
        InvolvedInGame,

        /// <summary>
        /// Running a simul match
        /// </summary>
        [ServerVariableName("~")]
        RunningSimulationMatch,

        /// <summary>
        /// Not open for a match
        /// </summary>
        [ServerVariableName(":")]
        NotOpenForMatch,

        /// <summary>
        /// Examining a game
        /// </summary>
        [ServerVariableName("#")]
        ExaminingGame,

        /// <summary>
        /// Inactive for 5 minutes or longer, or if "busy" is set not busy
        /// </summary>
        [ServerVariableName(".")]
        Inactive,

        /// <summary>
        /// Involved in a tournament
        /// </summary>
        [ServerVariableName("&")]
        InvolvedInTournament,
    }
}
