namespace FicsClientLibraryUnitTests
{
    using System.Diagnostics;
    using System.Threading.Tasks;

    public class TestsBase
    {
        public static T Wait<T>(Task<T> task, int millisecondsTimeout = 10000)
        {
            Debug.Assert(task.Wait(millisecondsTimeout));
            return task.Result;
        }

        public static void Wait(Task task, int millisecondsTimeout = 10000)
        {
            Debug.Assert(task.Wait(millisecondsTimeout));
        }
    }
}
