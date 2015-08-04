namespace Internet.Chess.Server.Fics
{
    using System;

    /// <summary>
    /// Attribute used for naming server variables/text values associated with enums/properties.
    /// </summary>
    internal class ServerVariableNameAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ServerVariableNameAttribute"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        public ServerVariableNameAttribute(string name)
        {
            Name = name;
        }

        /// <summary>
        /// Gets the server variable name.
        /// </summary>
        public string Name { get; private set; }
    }
}
