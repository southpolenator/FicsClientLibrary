namespace Internet.Chess.Server.Fics
{
    using System;

    [Flags]
    public enum AccountStatus
    {
        /// <summary>
        /// Regular account
        /// </summary>
        [ServerVariableName(" ")]
        RegularAccount = 0,

        /// <summary>
        /// Administrator account
        /// </summary>
        [ServerVariableName("*")]
        Administrator = 1,

        /// <summary>
        /// Blindfold account
        /// </summary>
        [ServerVariableName("B")]
        BlindfoldAccount = 2,

        /// <summary>
        /// Computer account
        /// </summary>
        [ServerVariableName("C")]
        ComputerAccount = 4,

        /// <summary>
        /// Team account
        /// </summary>
        [ServerVariableName("T")]
        TeamAccount = 8,

        /// <summary>
        /// Unregistered user
        /// </summary>
        [ServerVariableName("U")]
        UnregisteredUser = 16,

        /// <summary>
        /// Chess advisor
        /// </summary>
        [ServerVariableName("CA")]
        ChessAdvisor = 32,

        /// <summary>
        /// Service representative
        /// </summary>
        [ServerVariableName("SR")]
        ServiceRepresentative = 64,

        /// <summary>
        /// Tournament Director program or a bot of some sort
        /// </summary>
        [ServerVariableName("TD")]
        TournamentDirector = 128,

        /// <summary>
        /// Mamer manager
        /// </summary>
        [ServerVariableName("TM")]
        MamerManager = 256,

        /// <summary>
        /// FIDE Master
        /// </summary>
        [ServerVariableName("FM")]
        FideMaster = 512,

        /// <summary>
        /// International Master
        /// </summary>
        [ServerVariableName("IM")]
        InternationalMaster = 1024,

        /// <summary>
        /// Grand Master
        /// </summary>
        [ServerVariableName("GM")]
        GrandMaster = 2048,

        /// <summary>
        /// Women's FIDE Master
        /// </summary>
        [ServerVariableName("WFM")]
        WomenFideMaster = 4096,

        /// <summary>
        /// Women's International Master
        /// </summary>
        [ServerVariableName("WIM")]
        WomenInternationalMaster = 8192,

        /// <summary>
        /// Women's Grand Master
        /// </summary>
        [ServerVariableName("WGM")]
        WomenGrandMaster = 16384,

        /// <summary>
        /// Undocumented account status!!!
        /// </summary>
        [ServerVariableName("D")]
        Undocumented1 = 32768,
    }
}
