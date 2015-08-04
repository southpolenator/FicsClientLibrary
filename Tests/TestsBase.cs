namespace FicsClientLibraryTests
{
#if NETFX_CORE
    using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
#else
    using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif
    using System;
    using System.Threading.Tasks;

    public class TestsBase
    {
        public const int DefaultTestTimeout = 15000;

        public static T Wait<T>(Task<T> task, int millisecondsTimeout = int.MaxValue)
        {
            if (!task.Wait(millisecondsTimeout))
                throw new TimeoutException();
            return task.Result;
        }

        public static void Wait(Task task, int millisecondsTimeout = int.MaxValue)
        {
            if (!task.Wait(millisecondsTimeout))
                throw new TimeoutException();
        }

        public static void VerifyException(Task task, string exceptionString)
        {
            VerifyException<Exception>(task, exceptionString);
        }

        public static void VerifyException<TException>(Task task, string exceptionString)
        {
            try
            {
                task.Wait();
                Assert.Fail("Task passed without exception while it was expected to fail.");
            }
            catch (AggregateException ex)
            {
                Assert.AreEqual(typeof(TException), ex.InnerException.GetType());
                Assert.AreEqual(exceptionString, ex.InnerException.Message);
            }
            catch (Exception ex)
            {
                Assert.AreEqual(typeof(TException), ex.GetType());
                Assert.AreEqual(exceptionString, ex.Message);
            }
        }
    }
}
