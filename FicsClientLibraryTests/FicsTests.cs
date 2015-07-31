namespace FicsClientLibraryUnitTests
{
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
        [TestMethod, Timeout(DefaultTestTimeout)]
        public void FicsGuestLogin()
        {
            SetupGuestClient();
        }

        [TestMethod, Timeout(DefaultTestTimeout)]
        public void FicsListAllGames()
        {
            var games = Wait(SetupGuestClient().ListGames());

            Debug.Assert(games.Count > 0);
        }

        [TestMethod, Timeout(DefaultTestTimeout)]
        public void FicsReadServerLists()
        {
            var lists = Wait(SetupGuestClient().GetLists());

            Debug.Assert(lists.Count > 0);
        }

        [TestMethod, Timeout(DefaultTestTimeout)]
        public void FicsListBughouse()
        {
            var bughouse = Wait(SetupGuestClient().ListBughouse());

            Debug.Assert(bughouse.Games != null);
            Debug.Assert(bughouse.Partnerships != null);
            Debug.Assert(bughouse.Players != null);
        }

        [TestMethod, Timeout(DefaultTestTimeout)]
        public void FicsListPlayers()
        {
            var players = Wait(SetupGuestClient().ListPlayers());

            Debug.Assert(players.Count > 0);
        }

        [TestMethod, Timeout(DefaultTestTimeout)]
        public void FicsObserveGame()
        {
            FicsClient client = SetupGuestClient();
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
            client.ServerInterfaceVariables.PreciseTimes = true;
            client.ServerInterfaceVariables.DetailedGameInfo = true;
            client.ServerInterfaceVariables.PreMove = true;
            client.ServerInterfaceVariables.SmartMove = false;
            return client;
        }
    }
}
