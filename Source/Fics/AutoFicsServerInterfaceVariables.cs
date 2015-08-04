namespace Internet.Chess.Server.Fics
{
    internal class AutoFicsServerInterfaceVariables : AutoFicsServerVariablesBase, IFicsServerInterfaceVariables
    {
        public AutoFicsServerInterfaceVariables(FicsClient client, ServerVariablesBase variables, FicsCommand command)
            : base(client, variables, command)
        {
        }

        public bool CompressMove { get { return GetValue(); } set { SetValue(value); } }
        public bool DefaultPrompt { get { return GetValue(); } set { SetValue(value); } }
        public bool Lock { get { return GetValue(); } set { SetValue(value); } }
        public bool PreciseTimes { get { return GetValue(); } set { SetValue(value); } }
        public bool SeekRemove { get { return GetValue(); } set { SetValue(value); } }
        public int StartPosition { get { return GetValue(); } set { SetValue(value); } }
        public bool SendCommandsAsBlock { get { return GetValue(); } set { SetValue(value); } }
        public bool DetailedGameInfo { get { return GetValue(); } set { SetValue(value); } }
        public bool PendingInfo { get { return GetValue(); } set { SetValue(value); } }
        public bool Graph { get { return GetValue(); } set { SetValue(value); } }
        public bool SeekInfo { get { return GetValue(); } set { SetValue(value); } }
        public bool ExtAscii { get { return GetValue(); } set { SetValue(value); } }
        public bool ShowServer { get { return GetValue(); } set { SetValue(value); } }
        public bool NoHighlight { get { return GetValue(); } set { SetValue(value); } }
        public bool Vthighlight { get { return GetValue(); } set { SetValue(value); } }
        public bool Pin { get { return GetValue(); } set { SetValue(value); } }
        public bool PingInfo { get { return GetValue(); } set { SetValue(value); } }
        public bool BoardInfo { get { return GetValue(); } set { SetValue(value); } }
        public bool ExtUserInfo { get { return GetValue(); } set { SetValue(value); } }
        public bool AudioChat { get { return GetValue(); } set { SetValue(value); } }
        public bool SeekCa { get { return GetValue(); } set { SetValue(value); } }
        public bool ShowOwnSeek { get { return GetValue(); } set { SetValue(value); } }
        public bool PreMove { get { return GetValue(); } set { SetValue(value); } }
        public bool SmartMove { get { return GetValue(); } set { SetValue(value); } }
        public bool MoveCase { get { return GetValue(); } set { SetValue(value); } }
        public bool NoWrap { get { return GetValue(); } set { SetValue(value); } }
        public bool AllResults { get { return GetValue(); } set { SetValue(value); } }
        public bool SingleBoard { get { return GetValue(); } set { SetValue(value); } }
        public bool Suicide { get { return GetValue(); } set { SetValue(value); } }
        public bool CrazyHouse { get { return GetValue(); } set { SetValue(value); } }
        public bool Losers { get { return GetValue(); } set { SetValue(value); } }
        public bool WildCastle { get { return GetValue(); } set { SetValue(value); } }
        public bool FischerRandom { get { return GetValue(); } set { SetValue(value); } }
        public bool Atomic { get { return GetValue(); } set { SetValue(value); } }
        public bool Xml { get { return GetValue(); } set { SetValue(value); } }
    }
}
