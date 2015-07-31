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
    public class IcsClientTests : TestsBase
    {
        [TestMethod, Timeout(DefaultTestTimeout)]
        public void IcsGuestLogin()
        {
            IcsClient client = new IcsClient("freechess.org", 5000, "fics% ", "\n\r");
            int messagesReceived = 0;

            client.UnknownMessageReceived += (message) =>
                {
                    // We need to receive at least welcome message
                    messagesReceived++;
                };
            Wait(client.LoginGuest());
            Debug.Assert(messagesReceived > 0);
        }
    }
}
