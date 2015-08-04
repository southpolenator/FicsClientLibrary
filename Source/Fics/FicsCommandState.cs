using System.Threading;

namespace Internet.Chess.Server.Fics
{
    internal class FicsCommandState
    {
        private ManualResetEventSlim waitingEvent = new ManualResetEventSlim(true);

        public FicsCommand Command { get; set; }
        public bool IsExecuting
        {
            get
            {
                return !waitingEvent.IsSet;
            }

            set
            {
                if (value)
                {
                    waitingEvent.Reset();
                }
                else
                {
                    waitingEvent.Set();
                }
            }
        }

        public string Result { get; set; }

        public void WaitForEnd()
        {
            waitingEvent.Wait();
        }

        public int CommandCode
        {
            get
            {
                if (Command == FicsCommand.NotSet)
                {
                    return -1;
                }

                return Command.GetSingleAttribute<ServerCommandCodeAttribute>().Code;
            }
        }
    };
}
