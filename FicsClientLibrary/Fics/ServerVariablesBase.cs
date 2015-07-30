namespace Internet.Chess.Server.Fics
{
    using System;
    using System.Collections.Generic;
    using System.Threading;

    /// <summary>
    /// Base class for reading server variables
    /// </summary>
    internal class ServerVariablesBase
    {
        /// <summary>
        /// The waiting event
        /// </summary>
        private ManualResetEventSlim waitingEvent = new ManualResetEventSlim();

        /// <summary>
        /// Initializes the properties with given variables.
        /// </summary>
        /// <param name="variables">The variables.</param>
        public void Initialize(Dictionary<string, string> variables)
        {
            var properties = GetType().GetInterfaces()[0].GetProperties();

            foreach (var property in properties)
            {
                string value;

                if (variables.TryGetValue(property.GetSingleAttribute<ServerVariableNameAttribute>().Name, out value))
                {
                    switch (property.PropertyType.Name)
                    {
                        case "Int32":
                            property.SetValue(this, int.Parse(value));
                            break;
                        case "Boolean":
                            property.SetValue(this, value == "1");
                            break;
                        case "String":
                            property.SetValue(this, value);
                            break;
                        default:
                            throw new Exception("Unknown property type: " + property.PropertyType.Name);
                    }
                }
            }

            waitingEvent.Set();
        }

        /// <summary>
        /// Waits the initalization of properties.
        /// </summary>
        /// <remarks>This function is being used to insure we are not reading uninitialized properties</remarks>
        public void WaitInitalization()
        {
            waitingEvent.Wait();
        }
    }
}
