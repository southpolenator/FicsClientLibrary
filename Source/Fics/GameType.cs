namespace Internet.Chess.Server.Fics
{
    public enum GameType
    {
        [ServerVariableName("b")]
        Blitz,

        [ServerVariableName("l")]
        Lightning,

        [ServerVariableName("u")]
        Untimed,

        [ServerVariableName("e")]
        ExaminedGame,

        [ServerVariableName("s")]
        Standard,

        [ServerVariableName("w")]
        Wild,

        [ServerVariableName("x")]
        Atomic,

        [ServerVariableName("z")]
        Crazyhouse,

        [ServerVariableName("B")]
        Bughouse,

        [ServerVariableName("L")]
        Losers,

        [ServerVariableName("S")]
        Suicide,

        [ServerVariableName("n")]
        NonstandardGame,
    }
}
