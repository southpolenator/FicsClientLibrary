namespace Internet.Chess.Server
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Internet chess server client
    /// </summary>
    public class IcsClient
    {
        /// <summary>
        /// Delegate for processing unknown messages from the server.
        /// </summary>
        /// <param name="message">The message.</param>
        public delegate void UnknownMessageReceivedDelegate(string message);

        /// <summary>
        /// The background message reading and dispatching task
        /// </summary>
        private Task messageReadingTask;

        /// <summary>
        /// The cancellation token for background message reading task
        /// </summary>
        private CancellationTokenSource cancellationToken = new CancellationTokenSource();

        /// <summary>
        /// The telnet connection to the server
        /// </summary>
        private TelnetClient telnet;

        /// <summary>
        /// Initializes a new instance of the <see cref="IcsClient"/> class.
        /// </summary>
        /// <param name="server">The server address.</param>
        /// <param name="port">The server port.</param>
        /// <param name="prompt">The server prompt.</param>
        /// <param name="newLine">The server new line.</param>
        public IcsClient(string server, int port, string prompt, string newLine)
        {
            telnet = new TelnetClient(server, port, prompt, newLine);
            messageReadingTask = new Task(MessageReadingTask, cancellationToken.Token, TaskCreationOptions.LongRunning);
        }

        /// <summary>
        /// Occurs when unknown message is received from server.
        /// </summary>
        public event UnknownMessageReceivedDelegate UnknownMessageReceived;

        /// <summary>
        /// Gets the username of logged in user.
        /// </summary>
        public string Username { get { return telnet.Username; } }

        /// <summary>
        /// Logins the specified user.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="password">The password.</param>
        public async Task Login(string username, string password)
        {
            ProcessMessages(await telnet.Login(username, password));
            messageReadingTask.Start();
            LoginFinished();
        }

        /// <summary>
        /// Logins the guest account.
        /// </summary>
        public async Task LoginGuest()
        {
            await Login(TelnetClient.GuestUsername, "");
        }

        /// <summary>
        /// Sends the specified message to the server.
        /// </summary>
        /// <param name="message">The message.</param>
        public virtual async Task Send(string message)
        {
            await telnet.Write(message);
        }

        /// <summary>
        /// Sends the specified formatted message to the server.
        /// </summary>
        /// <param name="format">The message format.</param>
        /// <param name="args">The arguments.</param>
        public async Task Send(string format, params object[] args)
        {
            await Send(string.Format(format, args));
        }

        /// <summary>
        /// Event when login is finished.
        /// </summary>
        protected virtual void LoginFinished()
        {
            // Do nothing
        }

        /// <summary>
        /// Determines whether the specified message is known message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <returns><c>true</c> if it is known message, <c>false</c> otherwise</returns>
        internal virtual bool IsKnownMessage(ref string message)
        {
            return false;
        }

        /// <summary>
        /// Background task for reading and dispatching messages.
        /// </summary>
        private void MessageReadingTask()
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                ProcessMessages(telnet.Read());
            }
        }

        /// <summary>
        /// Processes the messages from server.
        /// </summary>
        /// <remarks>Server can return multiple messages at the same time, so splitting is done in this function.</remarks>
        /// <param name="received">The received.</param>
        private void ProcessMessages(string received)
        {
            string[] messages = received.Split(new string[] { "\n" + telnet.Prompt }, StringSplitOptions.RemoveEmptyEntries);

            foreach (string m in messages)
            {
                string message = m;

                // Process known messages
                if (!IsKnownMessage(ref message))
                {
                    // Emmit unprocessed messages
                    if (UnknownMessageReceived != null)
                    {
                        UnknownMessageReceived(message);
                    }
                }
            }
        }
    }
}
