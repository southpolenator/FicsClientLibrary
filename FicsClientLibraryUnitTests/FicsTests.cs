namespace FicsClientLibraryUnitTests
{
    using Internet.Chess.Server.Fics;
    using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
    using System.Diagnostics;

    [TestClass]
    public class FicsClientTests : TestsBase
    {
        [TestMethod]
        public void FicsGuestLogin()
        {
            FicsClient client = new FicsClient();

            int messagesReceived = 0;

            client.UnknownMessageReceived += (message) =>
            {
                // We need to receive at least welcome message
                messagesReceived++;
            };

            Wait(client.LoginGuest());
            Debug.Assert(messagesReceived > 0);

            Debug.WriteLine(client.ServerVariables.Time);
            client.ServerVariables.Time = 1;
        }
    }
}
