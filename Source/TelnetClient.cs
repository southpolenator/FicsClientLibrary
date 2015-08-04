namespace Internet.Chess.Server
{
    using System;
    using System.Runtime.InteropServices.WindowsRuntime;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using Windows.Networking;
    using Windows.Networking.Sockets;
    using Windows.Storage.Streams;

    /// <summary>
    /// Telnet client that communicates with server over socket connection.
    /// </summary>
    public class TelnetClient
    {
        /// <summary>
        /// The guest account
        /// </summary>
        public const string GuestUsername = "guest";

        /// <summary>
        /// The socket connection to the telnet server
        /// </summary>
        private StreamSocket socket = new StreamSocket();

        /// <summary>
        /// The semaphore used for writing to the socket
        /// </summary>
        private SemaphoreSlim writeSemaphore = new SemaphoreSlim(1, 1);

        /// <summary>
        /// Initializes a new instance of the <see cref="TelnetClient"/> class.
        /// </summary>
        /// <param name="server">The server address.</param>
        /// <param name="port">The server port.</param>
        /// <param name="prompt">The server prompt.</param>
        /// <param name="newLine">The server new line.</param>
        /// <param name="encoding">The server encoding.</param>
        public TelnetClient(string server, int port, string prompt = "", string newLine = "\n\r", Encoding encoding = null)
        {
            socket.ConnectAsync(new HostName(server), port.ToString()).AsTask().Wait();
            Prompt = prompt;
            Encoding = encoding != null ? encoding : Encoding.UTF8;
            NewLine = newLine;
        }

        /// <summary>
        /// Gets the username of logged in user.
        /// </summary>
        public string Username { get; private set; }

        /// <summary>
        /// Gets or sets the server prompt.
        /// </summary>
        public string Prompt { get; set; }

        /// <summary>
        /// Gets or sets the server encoding.
        /// </summary>
        public Encoding Encoding { get; set; }

        /// <summary>
        /// Gets or sets the server new line.
        /// </summary>
        public string NewLine { get; set; }

        /// <summary>
        /// Logins the specified user.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="password">The password.</param>
        /// <returns>Welcome message from the server</returns>
        public async Task<string> Login(string username, string password)
        {
            var prompt = this.Prompt;

            Prompt = "login: ";
            string usernameMessage = Read();
            await Write(username);
            Username = username;
            if (username != GuestUsername)
            {
                Prompt = "password: ";
                string passwordMessage = Read();
                await Write(password);
            }
            else
            {
                Prompt = "";
                string passwordMessage = Read();
                int quotesEnd = passwordMessage.LastIndexOf('"');
                int quotesBegin = passwordMessage.LastIndexOf('"', quotesEnd - 1);

                password = "";
                if (quotesBegin >= 0 && quotesEnd > 0)
                {
                    Username = passwordMessage.Substring(quotesBegin + 1, quotesEnd - quotesBegin - 1);
                }

                await Write(password);
            }

            Prompt = prompt;
            string loginMessage = Read();

            return loginMessage;
        }

        /// <summary>
        /// Logins the guest account.
        /// </summary>
        /// <returns>Welcome message from the server</returns>
        public async Task<string> LoginGuest()
        {
            return await Login(GuestUsername, "");
        }

        /// <summary>
        /// Reads message from the server.
        /// </summary>
        /// <remarks>This function is NOT thread safe.</remarks>
        /// <returns>Message read from the server</returns>
        public string Read()
        {
            string result = "";
            byte[] buffer = new byte[10240];
            int position = 0;

            while (true)
            {
                if (buffer.Length == position)
                {
                    Array.Resize(ref buffer, buffer.Length * 2);
                }

                int size = buffer.Length - position;
                IBuffer inputBuffer = buffer.AsBuffer(position, size);
                IBuffer read = socket.InputStream.ReadAsync(inputBuffer, (uint)size, InputStreamOptions.Partial).AsTask().Result;

                read.CopyTo(inputBuffer);
                position += (int)read.Length;
                result = new string(Encoding.GetChars(buffer, 0, position));
                if (string.IsNullOrEmpty(Prompt) || result.EndsWith(Prompt))
                {
                    break;
                }
            }

            result = result.Replace(NewLine, "\n");
            return result;
        }

        /// <summary>
        /// Writes the specified message to the server.
        /// </summary>
        /// <remarks>This function is thread safe.</remarks>
        /// <param name="message">The message.</param>
        public async Task Write(string message)
        {
            byte[] buffer = Encoding.GetBytes((message + NewLine).ToCharArray());

            await writeSemaphore.WaitAsync();
            try
            {
                await socket.OutputStream.WriteAsync(buffer.AsBuffer());
                await socket.OutputStream.FlushAsync();
            }
            finally
            {
                writeSemaphore.Release();
            }
        }
    }
}
