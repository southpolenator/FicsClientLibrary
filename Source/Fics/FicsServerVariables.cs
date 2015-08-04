namespace Internet.Chess.Server.Fics
{
    internal class FicsServerVariables : ServerVariablesBase, IFicsServerVariables
    {
        public int Time { get; set; }
        public int Increment { get; set; }
        public bool Rated { get; set; }
        public bool Open { get; set; }
        public bool Private { get; set; }
        public bool Shout { get; set; }
        public bool Pin { get; set; }
        public int Style { get; set; }
        public bool JPrivate { get; set; }
        public bool CShout { get; set; }
        public bool NotifiedBy { get; set; }
        public bool Flip { get; set; }
        public bool Kibitz { get; set; }
        public bool AvailableInfo { get; set; }
        public bool Highlight { get; set; }
        public bool Automail { get; set; }
        public int KibLevel { get; set; }
        public int AvailableMin { get; set; }
        public bool MoveBell { get; set; }
        public bool Pgn { get; set; }
        public bool AllowTellFromUnregisteredUsers { get; set; }
        public bool AllowChannelTellFromUnregisteredUsers { get; set; }
        public int AvailableMax { get; set; }
        public int Width { get; set; }
        public bool BugOpen { get; set; }
        public bool Gin { get; set; }
        public int Height { get; set; }
        public bool MailMess { get; set; }
        public bool Seek { get; set; }
        public bool ShowPromptTime { get; set; }
        public bool Tourney { get; set; }
        public bool MessageReply { get; set; }
        public bool BlockMessagesFromChannels { get; set; }
        public bool ShowOwnSeek { get; set; }
        public bool ShowProvisionalRatings { get; set; }
        public bool InGameSilence { get; set; }
        public bool AutoFlag { get; set; }
        public bool Unobserve { get; set; }
        public bool Echo { get; set; }
        public bool Examine { get; set; }
        public int MinMoveTime { get; set; }
        public int Tolerance { get; set; }
        public bool NoEscape { get; set; }
        public bool NoTakeBack { get; set; }
        public string TZone { get; set; }
        public string Language { get; set; }
        public string Prompt { get; set; }
        public string Formula { get; set; }
        public string Interface { get; set; }
    }
}
