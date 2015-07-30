namespace Internet.Chess.Server.Fics
{
    using System;

    /// <summary>
    /// Attribute used for server command codes associated with enums.
    /// </summary>
    internal class ServerCommandCodeAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ServerCommandCodeAttribute"/> class.
        /// </summary>
        /// <param name="code">The code.</param>
        public ServerCommandCodeAttribute(int code)
        {
            Code = code;
        }

        /// <summary>
        /// Gets the server command code.
        /// </summary>
        public int Code { get; private set; }
    }
}
