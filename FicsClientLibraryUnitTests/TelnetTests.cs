namespace FicsClientLibraryUnitTests
{
    using Internet.Chess.Server;
    using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
    using System.Diagnostics;
    using System.Threading.Tasks;

    [TestClass]
    public class TelnetTests : TestsBase
    {
        [TestMethod]
        public void GuestLogin()
        {
            TelnetClient client = new TelnetClient("freechess.org", 5000, "fics% ");

            Debug.Assert(!string.IsNullOrEmpty(Wait(client.LoginGuest())));
        }
    }
}
