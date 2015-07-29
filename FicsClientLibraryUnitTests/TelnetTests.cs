namespace FicsClientLibraryUnitTests
{
    using Internet.Chess.Server;
    using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
    using System.Diagnostics;
    using System.Threading.Tasks;

    [TestClass]
    public class TelnetTests
    {
        [TestMethod]
        public void GuestLogin()
        {
            TelnetClient client = new TelnetClient("freechess.org", 5000, "fics% ");

            Debug.Assert(!string.IsNullOrEmpty(Wait(client.LoginGuest())));
        }

        private static T Wait<T>(Task<T> task, int millisecondsTimeout = 10000)
        {
            Debug.Assert(task.Wait(millisecondsTimeout));
            return task.Result;
        }
    }
}
