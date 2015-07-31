namespace FicsClientLibraryTests
{
    using Internet.Chess.Server;
#if NETFX_CORE
    using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
#else
    using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif
    using System.Diagnostics;

    [TestClass]
    public class TelnetTests : TestsBase
    {
        [TestMethod, Timeout(DefaultTestTimeout)]
        public void TelnetGuestLogin()
        {
            TelnetClient client = new TelnetClient("freechess.org", 5000, "fics% ");

            string welcomeMessage = Wait(client.LoginGuest());
            Debug.Assert(!string.IsNullOrEmpty(welcomeMessage));
        }
    }
}
