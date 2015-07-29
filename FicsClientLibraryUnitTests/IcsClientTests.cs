namespace FicsClientLibraryUnitTests
{
    using Internet.Chess.Server;
    using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
    using System.Diagnostics;

    [TestClass]
    public class IcsClientTests : TestsBase
    {
        [TestMethod]
        public void GuestLogin()
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
