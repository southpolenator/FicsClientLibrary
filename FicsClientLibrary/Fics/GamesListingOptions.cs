namespace Internet.Chess.Server.Fics
{
    using System;

    [Flags]
    public enum GamesListingOptions
    {
        [ServerVariableName("b")]
        Blitz = 1,

        [ServerVariableName("l")]
        Lightning = 2,

        [ServerVariableName("u")]
        Untimed = 4,

        [ServerVariableName("e")]
        ExaminedGame = 8,

        [ServerVariableName("s")]
        Standard = 16,

        [ServerVariableName("w")]
        Wild = 32,

        [ServerVariableName("x")]
        Atomic = 64,

        [ServerVariableName("z")]
        Crazyhouse = 128,

        [ServerVariableName("B")]
        Bughouse = 256,

        [ServerVariableName("L")]
        Losers = 512,

        [ServerVariableName("S")]
        Suicide = 1024,
    }
}
