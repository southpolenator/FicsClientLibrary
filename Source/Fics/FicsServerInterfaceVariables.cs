namespace Internet.Chess.Server.Fics
{
    internal class FicsServerInterfaceVariables : ServerVariablesBase, IFicsServerInterfaceVariables
    {
        public bool CompressMove { get; set; }
        public bool DefaultPrompt { get; set; }
        public bool Lock { get; set; }
        public bool PreciseTimes { get; set; }
        public bool SeekRemove { get; set; }
        public int StartPosition { get; set; }
        public bool SendCommandsAsBlock { get; set; }
        public bool DetailedGameInfo { get; set; }
        public bool PendingInfo { get; set; }
        public bool Graph { get; set; }
        public bool SeekInfo { get; set; }
        public bool ExtAscii { get; set; }
        public bool ShowServer { get; set; }
        public bool NoHighlight { get; set; }
        public bool Vthighlight { get; set; }
        public bool Pin { get; set; }
        public bool PingInfo { get; set; }
        public bool BoardInfo { get; set; }
        public bool ExtUserInfo { get; set; }
        public bool AudioChat { get; set; }
        public bool SeekCa { get; set; }
        public bool ShowOwnSeek { get; set; }
        public bool PreMove { get; set; }
        public bool SmartMove { get; set; }
        public bool MoveCase { get; set; }
        public bool NoWrap { get; set; }
        public bool AllResults { get; set; }
        public bool SingleBoard { get; set; }
        public bool Suicide { get; set; }
        public bool CrazyHouse { get; set; }
        public bool Losers { get; set; }
        public bool WildCastle { get; set; }
        public bool FischerRandom { get; set; }
        public bool Atomic { get; set; }
        public bool Xml { get; set; }
    }
}
