namespace FicsClientLibraryUnitTests
{
    using Internet.Chess.Server;
    using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
    using System.Diagnostics;
    using System.Threading.Tasks;

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
