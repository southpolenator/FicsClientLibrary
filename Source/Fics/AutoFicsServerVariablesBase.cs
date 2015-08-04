namespace Internet.Chess.Server.Fics
{
    using System.Runtime.CompilerServices;

    internal class AutoFicsServerVariablesBase
    {
        private FicsClient client;
        private ServerVariablesBase variables;
        private FicsCommand command;

        public AutoFicsServerVariablesBase(FicsClient client, ServerVariablesBase variables, FicsCommand command)
        {
            this.client = client;
            this.variables = variables;
            this.command = command;
        }

        protected dynamic GetValue([CallerMemberName]string propertyName = null)
        {
            variables.WaitInitalization();
            return variables.GetType().GetProperty(propertyName).GetValue(variables);
        }

        protected void SetValue(object value, [CallerMemberName]string propertyName = null)
        {
            variables.WaitInitalization();

            var property = variables.GetType().GetInterfaces()[0].GetProperty(propertyName);
            string variableName = property.GetSingleAttribute<ServerVariableNameAttribute>().Name;

            client.Send(command, variableName, value).Wait();
            property.SetValue(variables, value);
        }
    }
}
