﻿namespace FicsClientLibraryUnitTests
{
    using System;
    using System.Threading.Tasks;

    public class TestsBase
    {
        public const int DefaultTestTimeout = 15000;

        public static T Wait<T>(Task<T> task, int millisecondsTimeout = 15000)
        {
            if (!task.Wait(millisecondsTimeout))
                throw new TimeoutException();
            return task.Result;
        }

        public static void Wait(Task task, int millisecondsTimeout = 15000)
        {
            if (!task.Wait(millisecondsTimeout))
                throw new TimeoutException();
        }
    }
}
