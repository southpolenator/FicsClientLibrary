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
            this.socket.ConnectAsync(new HostName(server), port.ToString()).AsTask().Wait();
            this.Prompt = prompt;
            this.Encoding = encoding != null ? encoding : Encoding.UTF8;
            this.NewLine = newLine;
        }

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

            this.Prompt = "login: ";
            string usernameMessage = await Read();
            await Write(username);
            if (username != GuestUsername)
            {
                this.Prompt = "password: ";
            }
            else
            {
                this.Prompt = "";
                password = "";
            }

            string passwordMessage = await Read();
            await Write(password);
            this.Prompt = prompt;
            string loginMessage = await Read();

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
        public async Task<string> Read()
        {
            const int ReadingBufferSize = 10240;
            byte[] buffer = new byte[0];
            string result = "";

            using (DataReader reader = new DataReader(socket.InputStream))
            {
                reader.InputStreamOptions = InputStreamOptions.Partial;
                await reader.LoadAsync(ReadingBufferSize);
                while (reader.UnconsumedBufferLength > 0)
                {
                    int position = buffer.Length;
                    int count = (int)reader.UnconsumedBufferLength;

                    Array.Resize(ref buffer, buffer.Length + count);
                    reader.ReadBuffer(reader.UnconsumedBufferLength).CopyTo(0, buffer, position, count);

                    result = new string(this.Encoding.GetChars(buffer));
                    if (string.IsNullOrEmpty(this.Prompt) || result.EndsWith(this.Prompt))
                    {
                        break;
                    }

                    await reader.LoadAsync(ReadingBufferSize);
                }

                reader.DetachStream();
                result = result.Replace(NewLine, "\n");
                return result;
            }
        }

        /// <summary>
        /// Writes the specified message to the server.
        /// </summary>
        /// <remarks>This function is thread safe.</remarks>
        /// <param name="message">The message.</param>
        public async Task Write(string message)
        {
            byte[] buffer = this.Encoding.GetBytes((message + NewLine).ToCharArray());

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
