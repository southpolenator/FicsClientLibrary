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
    using System.Threading;

    [TestClass]
    public class FicsClientTests : TestsBase
    {
        private static FicsClient client;
        private static TestContext context;

        [ClassInitialize, Timeout(DefaultTestTimeout)]
        public static void ClassInitialize(TestContext context)
        {
            client = SetupGuestClient();
            FicsClientTests.context = context;
        }

        [TestCleanup, Timeout(DefaultTestTimeout)]
        public void TestCleanup()
        {
            if (context.CurrentTestOutcome != UnitTestOutcome.Passed)
            {
                ClassInitialize(context);
            }
        }

        [TestMethod, Timeout(DefaultTestTimeout)]
        public void FicsGuestLogin()
        {
            Assert.IsNotNull(client);
        }

        [TestMethod, Timeout(DefaultTestTimeout)]
        public void FicsListAllGames()
        {
            var games = Wait(client.ListGames());

            Assert.IsTrue(games.Count > 0);
        }

        [TestMethod, Timeout(DefaultTestTimeout)]
        public void FicsReadServerLists()
        {
            var lists = Wait(client.GetLists());

            Assert.IsTrue(lists.Count > 0);
        }

        [TestMethod, Timeout(DefaultTestTimeout)]
        public void FicsListBughouse()
        {
            var bughouse = Wait(client.ListBughouse());

            Assert.IsNotNull(bughouse.Games);
            Assert.IsNotNull(bughouse.Partnerships);
            Assert.IsNotNull(bughouse.Players);
        }

        [TestMethod, Timeout(DefaultTestTimeout)]
        public void FicsListPlayers()
        {
            var players = Wait(client.ListPlayers());

            Assert.IsTrue(players.Count > 0);
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
            var observeGameResult = Wait(client.StartObservingGame(game));

            Assert.AreEqual(observeGameResult.GameInfo.GameId, game.Id);

            if (observeGameResult.GameInfo.PartnersGameId > 0)
            {
                var observePartnerGameResult = Wait(client.StartObservingGame(observeGameResult.GameInfo.PartnersGameId));

                Assert.AreEqual(observePartnerGameResult.GameInfo.GameId, observeGameResult.GameInfo.PartnersGameId);

                Wait(client.StopObservingGame(observePartnerGameResult.GameInfo.GameId));
            }

            Wait(client.StopObservingGame(game));
        }

        [TestMethod, Timeout(DefaultTestTimeout)]
        public void FicsFollowPlayerPlayingGame()
        {
            var games = Wait(client.ListGames());
            Game game = games.FirstOrDefault(g => g.Type == GameType.Bughouse);
            game = game ?? games.FirstOrDefault(g => g.Type == GameType.Crazyhouse);
            game = game ?? games[new Random().Next(games.Count)];
            var observeGameResult = Wait(client.StartFollowingPlayer(game.WhitePlayer));

            Assert.AreEqual(observeGameResult.GameInfo.GameId, game.Id);

            if (observeGameResult.GameInfo.PartnersGameId > 0)
            {
                var observePartnerGameResult = Wait(client.StartObservingGame(observeGameResult.GameInfo.PartnersGameId));

                Assert.AreEqual(observePartnerGameResult.GameInfo.GameId, observeGameResult.GameInfo.PartnersGameId);

                Wait(client.StopObservingGame(observePartnerGameResult.GameInfo.GameId));
            }

            Wait(client.StopFollowingPlayer());
        }

        [TestMethod, Timeout(DefaultTestTimeout)]
        public void FicsFollowPlayerNotPlayingGame()
        {
            var users = Wait(client.ListPlayers());
            var user = users.FirstOrDefault(u => u.Username != client.Username && u.Status != PlayerStatus.ExaminingGame && u.Status != PlayerStatus.InvolvedInGame && u.Status != PlayerStatus.RunningSimulationMatch);
            var observeGameResult = Wait(client.StartFollowingPlayer(user.Username));

            Assert.IsNull(observeGameResult);
            Wait(client.StopFollowingPlayer());
        }

        [TestMethod, Timeout(DefaultTestTimeout)]
        public void FicsSendMessage()
        {
            // Verify that message can be sent
            const string messageText = "Test message";
            FicsClient secondClient = SetupGuestClient();
            string username = null, message = null;
            ManualResetEventSlim messageWaiting = new ManualResetEventSlim();

            secondClient.MessageReceived += (u, m) =>
            {
                username = u;
                message = m;
                messageWaiting.Set();
            };
            Wait(client.SendMessage(secondClient.Username, messageText));
            messageWaiting.Wait();
            Assert.AreEqual(username, client.Username);
            Assert.AreEqual(message, messageText);

            // Block (censor) user from sending message
            secondClient.CensoredList.Add(client.Username);
            VerifyException(client.SendMessage(secondClient.Username, messageText), "Player \"" + secondClient.Username + "\" is censoring you.\n");
            secondClient.CensoredList.Remove(client.Username);
        }

        [TestMethod, Timeout(DefaultTestTimeout)]
        public void FicsChannelMessage()
        {
            VerifyException(client.SendMessage(100, "I'm bored"), "Only registered users may send tells to channels other than 4, 7 and 53.\n");
        }

        [TestMethod, Timeout(DefaultTestTimeout)]
        public void FicsShoutMessage()
        {
            VerifyException(client.SendShoutMessage("I'm bored"), "Only registered players can use the shout command.\n");
        }

        [TestMethod, Timeout(DefaultTestTimeout)]
        public void FicsChessShoutMessage()
        {
            VerifyException(client.SendChessShoutMessage("I'm bored"), "Only registered players can use the cshout command.\n");
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
            client.ServerVariables.Seek = false;
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
