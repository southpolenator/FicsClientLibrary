namespace Internet.Chess.Server.Fics
{
    using System;

    /// <summary>
    /// Attribute used for naming server commands associated with enums.
    /// </summary>
    internal class ServerCommandNameAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ServerCommandNameAttribute"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        public ServerCommandNameAttribute(string name)
        {
            Name = name;
        }

        /// <summary>
        /// Gets the server command name.
        /// </summary>
        public string Name { get; private set; }
    }
}
