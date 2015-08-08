namespace FicsClientLibraryTests
{
    using Internet.Chess.Server;
    using Internet.Chess.Server.Fics;
#if NETFX_CORE
    using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
#else
    using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif
    using System;
    using System.Diagnostics;
    using System.Linq;

    [TestClass]
    public class FicsParsingTests : TestsBase
    {
        [TestMethod, Timeout(DefaultTestTimeout)]
        public void FicsParsePartnership()
        {
            var parthership = FicsClient.ParsePartnership("1801 ^VladimirPutin / 2008 .Liverppol", true);

            Assert.IsNotNull(parthership);
            Assert.AreEqual(parthership.Player1.Rating, 1801);
            Assert.AreEqual(parthership.Player1.Username, "VladimirPutin");
            Assert.AreEqual(parthership.Player2.Rating, 2008);
            Assert.AreEqual(parthership.Player2.Username, "Liverppol");
        }

        [TestMethod, Timeout(DefaultTestTimeout)]
        public void FicsParseGameEnded()
        {
            string GameEndedMessage = "\n{Game 457 (szesze vs. aldobaral) aldobaral forfeits on time} 1-0";
            FicsClient client = new FicsClient();
            GameEndedInfo gameEndedInfo = null;

            client.GameEnded += (info) =>
            {
                gameEndedInfo = info;
            };
            client.IsKnownMessage(ref GameEndedMessage);
            Assert.IsNotNull(gameEndedInfo);
            Assert.AreEqual(gameEndedInfo.GameId, 457);
            Assert.AreEqual(gameEndedInfo.WhitePlayerUsername, "szesze");
            Assert.AreEqual(gameEndedInfo.BlackPlayerUsername, "aldobaral");
            Assert.AreEqual(gameEndedInfo.Message, "aldobaral forfeits on time");
            Assert.AreEqual(gameEndedInfo.WhitePlayerPoints, 1);
            Assert.AreEqual(gameEndedInfo.BlackPlayerPoints, 0);
        }
    }
}
