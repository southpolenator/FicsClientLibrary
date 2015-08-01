namespace Internet.Chess.Server.Fics
{
    internal class AutoFicsServerVariables : AutoFicsServerVariablesBase, IFicsServerVariables
    {
        public AutoFicsServerVariables(FicsClient client, ServerVariablesBase variables, FicsCommand command)
            : base(client, variables, command)
        {
        }

        public int Time { get { return GetValue(); } set { SetValue(value); } }
        public int Increment { get { return GetValue(); } set { SetValue(value); } }
        public bool Rated { get { return GetValue(); } set { SetValue(value); } }
        public bool Open { get { return GetValue(); } set { SetValue(value); } }
        public bool Private { get { return GetValue(); } set { SetValue(value); } }
        public bool Shout { get { return GetValue(); } set { SetValue(value); } }
        public bool Pin { get { return GetValue(); } set { SetValue(value); } }
        public int Style { get { return GetValue(); } set { SetValue(value); } }
        public bool JPrivate { get { return GetValue(); } set { SetValue(value); } }
        public bool CShout { get { return GetValue(); } set { SetValue(value); } }
        public bool NotifiedBy { get { return GetValue(); } set { SetValue(value); } }
        public bool Flip { get { return GetValue(); } set { SetValue(value); } }
        public bool Kibitz { get { return GetValue(); } set { SetValue(value); } }
        public bool AvailableInfo { get { return GetValue(); } set { SetValue(value); } }
        public bool Highlight { get { return GetValue(); } set { SetValue(value); } }
        public bool Automail { get { return GetValue(); } set { SetValue(value); } }
        public int KibLevel { get { return GetValue(); } set { SetValue(value); } }
        public int AvailableMin { get { return GetValue(); } set { SetValue(value); } }
        public bool MoveBell { get { return GetValue(); } set { SetValue(value); } }
        public bool Pgn { get { return GetValue(); } set { SetValue(value); } }
        public bool AllowTellFromUnregisteredUsers { get { return GetValue(); } set { SetValue(value); } }
        public bool AllowChannelTellFromUnregisteredUsers { get { return GetValue(); } set { SetValue(value); } }
        public int AvailableMax { get { return GetValue(); } set { SetValue(value); } }
        public int Width { get { return GetValue(); } set { SetValue(value); } }
        public bool BugOpen { get { return GetValue(); } set { SetValue(value); } }
        public bool Gin { get { return GetValue(); } set { SetValue(value); } }
        public int Height { get { return GetValue(); } set { SetValue(value); } }
        public bool MailMess { get { return GetValue(); } set { SetValue(value); } }
        public bool Seek { get { return GetValue(); } set { SetValue(value); } }
        public bool ShowPromptTime { get { return GetValue(); } set { SetValue(value); } }
        public bool Tourney { get { return GetValue(); } set { SetValue(value); } }
        public bool MessageReply { get { return GetValue(); } set { SetValue(value); } }
        public bool BlockMessagesFromChannels { get { return GetValue(); } set { SetValue(value); } }
        public bool ShowOwnSeek { get { return GetValue(); } set { SetValue(value); } }
        public bool ShowProvisionalRatings { get { return GetValue(); } set { SetValue(value); } }
        public bool InGameSilence { get { return GetValue(); } set { SetValue(value); } }
        public bool AutoFlag { get { return GetValue(); } set { SetValue(value); } }
        public bool Unobserve { get { return GetValue(); } set { SetValue(value); } }
        public bool Echo { get { return GetValue(); } set { SetValue(value); } }
        public bool Examine { get { return GetValue(); } set { SetValue(value); } }
        public int MinMoveTime { get { return GetValue(); } set { SetValue(value); } }
        public int Tolerance { get { return GetValue(); } set { SetValue(value); } }
        public bool NoEscape { get { return GetValue(); } set { SetValue(value); } }
        public bool NoTakeBack { get { return GetValue(); } set { SetValue(value); } }
        public string TZone { get { return GetValue(); } set { SetValue(value); } }
        public string Language { get { return GetValue(); } set { SetValue(value); } }
        public string Prompt { get { return GetValue(); } set { SetValue(value); } }
        public string Formula { get { return GetValue(); } set { SetValue(value); } }
        public string Interface { get { return GetValue(); } set { SetValue(value); } }
    }
}
