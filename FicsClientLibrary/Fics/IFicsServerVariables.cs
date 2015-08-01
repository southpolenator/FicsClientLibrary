namespace Internet.Chess.Server.Fics
{
    public interface IFicsServerVariables
    {
        [ServerVariableName("time")]
        int Time { get; set; }

        [ServerVariableName("inc")]
        int Increment { get; set; }

        [ServerVariableName("rated")]
        bool Rated { get; set; }

        [ServerVariableName("open")]
        bool Open { get; set; }

        [ServerVariableName("private")]
        bool Private { get; set; }

        [ServerVariableName("shout")]
        bool Shout { get; set; }

        [ServerVariableName("pin")]
        bool Pin { get; set; }

        [ServerVariableName("style")]
        int Style { get; set; }

        [ServerVariableName("jprivate")]
        bool JPrivate { get; set; }

        [ServerVariableName("cshout")]
        bool CShout { get; set; }

        [ServerVariableName("notifiedby")]
        bool NotifiedBy { get; set; }

        [ServerVariableName("flip")]
        bool Flip { get; set; }

        [ServerVariableName("kibitz")]
        bool Kibitz { get; set; }

        [ServerVariableName("availinfo")]
        bool AvailableInfo { get; set; }

        [ServerVariableName("highlight")]
        bool Highlight { get; set; }

        [ServerVariableName("automail")]
        bool Automail { get; set; }

        [ServerVariableName("kiblevel")]
        int KibLevel { get; set; }

        [ServerVariableName("availmin")]
        int AvailableMin { get; set; }

        [ServerVariableName("bell")]
        bool MoveBell { get; set; }

        [ServerVariableName("pgn")]
        bool Pgn { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether you will receive tell messages from unregistered users.
        /// </summary>
        [ServerVariableName("tell")]
        bool AllowTellFromUnregisteredUsers { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether you will receive channel tell messages from unregistered users.
        /// </summary>
        [ServerVariableName("ctell")]
        bool AllowChannelTellFromUnregisteredUsers { get; set; }

        [ServerVariableName("availmax")]
        int AvailableMax { get; set; }

        [ServerVariableName("width")]
        int Width { get; set; }

        [ServerVariableName("bugopen")]
        bool BugOpen { get; set; }

        [ServerVariableName("gin")]
        bool Gin { get; set; }

        [ServerVariableName("height")]
        int Height { get; set; }

        [ServerVariableName("mailmess")]
        bool MailMess { get; set; }

        [ServerVariableName("seek")]
        bool Seek { get; set; }

        [ServerVariableName("ptime")]
        bool ShowPromptTime { get; set; }

        [ServerVariableName("tourney")]
        bool Tourney { get; set; }

        [ServerVariableName("messreply")]
        bool MessageReply { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether you block receiving channel tell messages.
        /// </summary>
        [ServerVariableName("chanoff")]
        bool BlockMessagesFromChannels { get; set; }

        [ServerVariableName("showownseek")]
        bool ShowOwnSeek { get; set; }

        [ServerVariableName("provshow")]
        bool ShowProvisionalRatings { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether you want silence during the game.
        /// Setting silence to <c>true</c> will turn off global messages (shouts, cshouts and channel tells)
        /// while you play, examine or observe a game so that communications to you will be less distracting
        /// to you. When you are not playing, examining or observing a game, you will hear global messages again.
        /// </summary>
        [ServerVariableName("silence")]
        bool InGameSilence { get; set; }

        [ServerVariableName("autoflag")]
        bool AutoFlag { get; set; }

        [ServerVariableName("unobserve")]
        bool Unobserve { get; set; }

        [ServerVariableName("echo")]
        bool Echo { get; set; }

        [ServerVariableName("examine")]
        bool Examine { get; set; }

        [ServerVariableName("minmovetime")]
        int MinMoveTime { get; set; }

        [ServerVariableName("tolerance")]
        int Tolerance { get; set; }

        [ServerVariableName("noescape")]
        bool NoEscape { get; set; }

        [ServerVariableName("notakeback")]
        bool NoTakeBack { get; set; }

        [ServerVariableName("tzone")]
        string TZone { get; set; }

        [ServerVariableName("Lang")]
        string Language { get; set; }

        [ServerVariableName("Prompt")]
        string Prompt { get; set; }

        [ServerVariableName("Formula")]
        string Formula { get; set; }

        [ServerVariableName("Interface")]
        string Interface { get; set; }
    }
}
