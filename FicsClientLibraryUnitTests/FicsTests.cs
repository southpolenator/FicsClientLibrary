namespace FicsClientLibraryUnitTests
{
    using Internet.Chess.Server.Fics;
    using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
    using System;
    using System.Diagnostics;

    [TestClass]
    public class FicsClientTests : TestsBase
    {
        [TestMethod, Timeout(DefaultTestTimeout)]
        public void FicsGuestLogin()
        {
            SetupGuestClient();
        }

        private static FicsClient SetupGuestClient()
        {
            return SetupClient((client) =>
            {
                Wait(client.LoginGuest());
            });
        }

        private static FicsClient SetupClient(string username, string password)
        {
            return SetupClient((client) =>
            {
                Wait(client.Login(username, password));
            });
        }

        private static FicsClient SetupClient(Action<FicsClient> loginAction)
        {
            FicsClient client = new FicsClient();

            client.UnknownMessageReceived += (message) =>
            {
                Debug.WriteLine(message);
            };

            loginAction(client);
            client.ServerVariables.ShowPromptTime = false;
            client.ServerVariables.MoveBell = false;
            client.ServerVariables.Style = 12;
            client.ServerVariables.ShowProvisionalRatings = true;
            client.ServerVariables.Interface = "FicsClientLibraryTests";
            client.ServerInterfaceVariables.PreciseTimes = true;
            client.ServerInterfaceVariables.DetailedGameInfo = true;
            client.ServerInterfaceVariables.PreMove = true;
            client.ServerInterfaceVariables.SmartMove = false;
            return client;
        }
    }
}
