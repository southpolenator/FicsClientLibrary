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

            Debug.Assert(parthership != null);
        }
    }
}
