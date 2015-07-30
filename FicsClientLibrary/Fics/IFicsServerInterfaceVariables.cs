namespace Internet.Chess.Server.Fics
{
    public interface IFicsServerInterfaceVariables
    {
        [ServerVariableName("compressmove")]
        bool CompressMove { get; set; }

        [ServerVariableName("defprompt")]
        bool DefaultPrompt { get; set; }

        [ServerVariableName("lock")]
        bool Lock { get; set; }

        [ServerVariableName("ms")]
        bool PreciseTimes { get; set; }

        [ServerVariableName("seekremove")]
        bool SeekRemove { get; set; }

        [ServerVariableName("startpos")]
        int StartPosition { get; set; }

        [ServerVariableName("block")]
        bool SendCommandsAsBlock { get; set; }

        [ServerVariableName("gameinfo")]
        bool DetailedGameInfo { get; set; }

        [ServerVariableName("pendinfo")]
        bool PendingInfo { get; set; }

        [ServerVariableName("graph")]
        bool Graph { get; set; }

        [ServerVariableName("seekinfo")]
        bool SeekInfo { get; set; }

        [ServerVariableName("extascii")]
        bool ExtAscii { get; set; }

        [ServerVariableName("showserver")]
        bool ShowServer { get; set; }

        [ServerVariableName("nohighlight")]
        bool NoHighlight { get; set; }

        [ServerVariableName("vthighlight")]
        bool Vthighlight { get; set; }

        [ServerVariableName("pin")]
        bool Pin { get; set; }

        [ServerVariableName("pinginfo")]
        bool PingInfo { get; set; }

        [ServerVariableName("boardinfo")]
        bool BoardInfo { get; set; }

        [ServerVariableName("extuserinfo")]
        bool ExtUserInfo { get; set; }

        [ServerVariableName("audiochat")]
        bool AudioChat { get; set; }

        [ServerVariableName("seekca")]
        bool SeekCa { get; set; }

        [ServerVariableName("showownseek")]
        bool ShowOwnSeek { get; set; }

        [ServerVariableName("premove")]
        bool PreMove { get; set; }

        [ServerVariableName("smartmove")]
        bool SmartMove { get; set; }

        [ServerVariableName("movecase")]
        bool MoveCase { get; set; }

        [ServerVariableName("nowrap")]
        bool NoWrap { get; set; }

        [ServerVariableName("allresults")]
        bool AllResults { get; set; }

        [ServerVariableName("singleboard")]
        bool SingleBoard { get; set; }

        [ServerVariableName("suicide")]
        bool Suicide { get; set; }

        [ServerVariableName("crazyhouse")]
        bool CrazyHouse { get; set; }

        [ServerVariableName("losers")]
        bool Losers { get; set; }

        [ServerVariableName("wildcastle")]
        bool WildCastle { get; set; }

        [ServerVariableName("fr")]
        bool FischerRandom { get; set; }

        [ServerVariableName("atomic")]
        bool Atomic { get; set; }

        [ServerVariableName("xml")]
        bool Xml { get; set; }
    }
}
