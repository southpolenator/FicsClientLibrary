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
    public class FicsClientTests : TestsBase
    {
        private static FicsClient client;

        [ClassInitialize, Timeout(DefaultTestTimeout)]
        public static void ClassInitialize(TestContext context)
        {
            client = SetupGuestClient();
        }

        [TestMethod, Timeout(DefaultTestTimeout)]
        public void FicsGuestLogin()
        {
            Debug.Assert(client != null);
        }

        [TestMethod, Timeout(DefaultTestTimeout)]
        public void FicsListAllGames()
        {
            var games = Wait(client.ListGames());

            Debug.Assert(games.Count > 0);
        }

        [TestMethod, Timeout(DefaultTestTimeout)]
        public void FicsReadServerLists()
        {
            var lists = Wait(client.GetLists());

            Debug.Assert(lists.Count > 0);
        }

        [TestMethod, Timeout(DefaultTestTimeout)]
        public void FicsListBughouse()
        {
            var bughouse = Wait(client.ListBughouse());

            Debug.Assert(bughouse.Games != null);
            Debug.Assert(bughouse.Partnerships != null);
            Debug.Assert(bughouse.Players != null);
        }

        [TestMethod, Timeout(DefaultTestTimeout)]
        public void FicsListPlayers()
        {
            var players = Wait(client.ListPlayers());

            Debug.Assert(players.Count > 0);
        }

        [TestMethod, Timeout(DefaultTestTimeout)]
        public void FicsTestServerVariables()
        {
            var serverVariables = client.ServerVariables;

            foreach (var property in serverVariables.GetType().GetProperties())
            {
                object value = property.GetValue(serverVariables);

                Debug.WriteLine("{0} = {1}", property.Name, value);
                if (value != null)
                {
                    property.SetValue(serverVariables, value);
                }
            }
        }

        [TestMethod, Timeout(DefaultTestTimeout)]
        public void FicsTestServerInterfaceVariables()
        {
            var serverInterfaceVariables = client.ServerInterfaceVariables;

            foreach (var property in serverInterfaceVariables.GetType().GetProperties())
            {
                object value = property.GetValue(serverInterfaceVariables);

                Debug.WriteLine("{0} = {1}", property.Name, value);
                if (value != null)
                {
                    property.SetValue(serverInterfaceVariables, value);
                }
            }
        }

        [TestMethod, Timeout(DefaultTestTimeout)]
        public void FicsObserveGame()
        {
            var games = Wait(client.ListGames());
            Game game = games.FirstOrDefault(g => g.Type == GameType.Bughouse);
            game = game ?? games.FirstOrDefault(g => g.Type == GameType.Crazyhouse);
            game = game ?? games[new Random().Next(games.Count)];
            var observeGameResult = Wait(client.ObserveGame(game));

            Debug.Assert(observeGameResult.GameInfo.GameId == game.Id);

            // TODO: Wait until game ends
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
            return SetupClient(async (client) =>
            {
                await client.Login(username, password);
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
            client.ServerVariables.Seek = false;
            client.ServerInterfaceVariables.PreciseTimes = true;
            client.ServerInterfaceVariables.DetailedGameInfo = true;
            client.ServerInterfaceVariables.PreMove = true;
            client.ServerInterfaceVariables.SmartMove = false;
            return client;
        }
    }
}
