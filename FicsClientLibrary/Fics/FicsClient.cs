namespace Internet.Chess.Server.Fics
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Reflection.Emit;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;

    public class FicsClient : IcsClient
    {
        public delegate void GameStateChangeDelegate(GameState gameState);

        internal abstract class AFicsServerVariables : IFicsServerVariables
        {
            public abstract int Time { get; set; }
            public abstract int Increment { get; set; }
            public abstract bool Rated { get; set; }
            public abstract bool Open { get; set; }
            public abstract bool Private { get; set; }
            public abstract bool Shout { get; set; }
            public abstract bool Pin { get; set; }
            public abstract int Style { get; set; }
            public abstract bool JPrivate { get; set; }
            public abstract bool CShout { get; set; }
            public abstract bool NotifiedBy { get; set; }
            public abstract bool Flip { get; set; }
            public abstract bool Kibitz { get; set; }
            public abstract bool AvailableInfo { get; set; }
            public abstract bool Highlight { get; set; }
            public abstract bool Automail { get; set; }
            public abstract int KibLevel { get; set; }
            public abstract int AvailableMin { get; set; }
            public abstract bool MoveBell { get; set; }
            public abstract bool Pgn { get; set; }
            public abstract bool Tell { get; set; }
            public abstract bool CTell { get; set; }
            public abstract int AvailableMax { get; set; }
            public abstract int Width { get; set; }
            public abstract bool BugOpen { get; set; }
            public abstract bool Gin { get; set; }
            public abstract int Height { get; set; }
            public abstract bool MailMess { get; set; }
            public abstract bool Seek { get; set; }
            public abstract bool ShowPromptTime { get; set; }
            public abstract bool Tourney { get; set; }
            public abstract bool MessageReply { get; set; }
            public abstract bool ChannelsOff { get; set; }
            public abstract bool ShowOwnSeek { get; set; }
            public abstract bool ShowProvisionalRatings { get; set; }
            public abstract bool Silence { get; set; }
            public abstract bool AutoFlag { get; set; }
            public abstract bool Unobserve { get; set; }
            public abstract bool Echo { get; set; }
            public abstract bool Examine { get; set; }
            public abstract int MinMoveTime { get; set; }
            public abstract int Tolerance { get; set; }
            public abstract bool NoEscape { get; set; }
            public abstract bool NoTakeBack { get; set; }
            public abstract string TZone { get; set; }
            public abstract string Language { get; set; }
            public abstract string Prompt { get; set; }
            public abstract string Formula { get; set; }
            public abstract string Interface { get; set; }
        }

        internal abstract class AFicsServerInterfaceVariables : IFicsServerInterfaceVariables
        {
            public abstract bool CompressMove { get; set; }
            public abstract bool DefaultPrompt { get; set; }
            public abstract bool Lock { get; set; }
            public abstract bool PreciseTimes { get; set; }
            public abstract bool SeekRemove { get; set; }
            public abstract int StartPosition { get; set; }
            public abstract bool SendCommandsAsBlock { get; set; }
            public abstract bool DetailedGameInfo { get; set; }
            public abstract bool PendingInfo { get; set; }
            public abstract bool Graph { get; set; }
            public abstract bool SeekInfo { get; set; }
            public abstract bool ExtAscii { get; set; }
            public abstract bool ShowServer { get; set; }
            public abstract bool NoHighlight { get; set; }
            public abstract bool Vthighlight { get; set; }
            public abstract bool Pin { get; set; }
            public abstract bool PingInfo { get; set; }
            public abstract bool BoardInfo { get; set; }
            public abstract bool ExtUserInfo { get; set; }
            public abstract bool AudioChat { get; set; }
            public abstract bool SeekCa { get; set; }
            public abstract bool ShowOwnSeek { get; set; }
            public abstract bool PreMove { get; set; }
            public abstract bool SmartMove { get; set; }
            public abstract bool MoveCase { get; set; }
            public abstract bool NoWrap { get; set; }
            public abstract bool AllResults { get; set; }
            public abstract bool SingleBoard { get; set; }
            public abstract bool Suicide { get; set; }
            public abstract bool CrazyHouse { get; set; }
            public abstract bool Losers { get; set; }
            public abstract bool WildCastle { get; set; }
            public abstract bool FischerRandom { get; set; }
            public abstract bool Atomic { get; set; }
            public abstract bool Xml { get; set; }
        }

        private class CommandState
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

        #region Constants
        public const string DefaultPrompt = "fics% ";
        public const string DefaultServer = "freechess.org";
        public const int DefaultServerPort = 5000;
        public const int AlternateServerPort = 23;
        public const string DefaultNewLine = "\n\r";

        public const char CommandBlockStart = (char)21;
        public const char CommandBlockSeparator = (char)22;
        public const char CommandBlockEnd = (char)23;
        public const char CommandBlockPoseStart = (char)24;
        public const char CommandBlockPoseEnd = (char)25;
        #endregion

        #region Cached values of server variables
        private FicsServerVariables variables = new FicsServerVariables();
        private FicsServerInterfaceVariables ivariables = new FicsServerInterfaceVariables();
        #endregion

        public FicsClient(string server = DefaultServer, int port = DefaultServerPort, string prompt = DefaultPrompt, string newLine = DefaultNewLine)
            : base(server, port, prompt, newLine)
        {
            ServerVariables = GenerateInterface<AFicsServerVariables>(variables, FicsCommand.SetServerVariable);
            ServerInterfaceVariables = GenerateInterface<AFicsServerInterfaceVariables>(ivariables, FicsCommand.SetServerInterfaceVariable);

            // Create standard server personal lists
            CensoredList = GetServerList(ServerList.CensoredList, isPublic: false);
            WontPlayList = GetServerList(ServerList.WontPlayList, isPublic: false);
            ListeningChannelsList = GetServerList(ServerList.ListeningChannelsList, isPublic: false);
        }

        #region Auto-generated interfaces for getting/settings server variables
        private static object assemblyGenerationLock = new object();
        private static ModuleBuilder moduleBuilder;
        private static Dictionary<string, Type> generatedTypes = new Dictionary<string, Type>();

        private TAbstractClass GenerateInterface<TAbstractClass>(ServerVariablesBase variables, FicsCommand command)
        {
            lock (assemblyGenerationLock)
            {
                if (moduleBuilder == null)
                {
                    var assemblyName = new AssemblyName("FicsClientGeneratedAssembly");
                    var assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);

                    moduleBuilder = assemblyBuilder.DefineDynamicModule("FicsClientGeneratedModule");
                }

                Type classType = typeof(TAbstractClass);
                string typeName = classType.Name + "_Generated";
                Type type = null;
                PropertyInfo[] classProperties = classType.GetInterfaces()[0].GetProperties();

                if (!generatedTypes.TryGetValue(typeName, out type))
                {
                    var typeBuilder = moduleBuilder.DefineType(typeName, TypeAttributes.Public | TypeAttributes.Class, classType);

                    foreach (var property in classProperties)
                    {
                        var actionType = Type.GetType("System.Action`1").MakeGenericType(property.PropertyType);
                        var functionType = Type.GetType("System.Func`1").MakeGenericType(property.PropertyType);
                        var setPropertyField = typeBuilder.DefineField("__set_" + property.Name, actionType, FieldAttributes.Public);
                        var getPropertyField = typeBuilder.DefineField("__get_" + property.Name, functionType, FieldAttributes.Public);

                        MethodBuilder methodBuilder = typeBuilder.DefineMethod("get_" + property.Name, MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.Virtual, property.PropertyType, new Type[0]);
                        typeBuilder.DefineMethodOverride(methodBuilder, property.GetMethod);
                        var ilg = methodBuilder.GetILGenerator();
                        ilg.Emit(OpCodes.Ldarg_0);
                        ilg.Emit(OpCodes.Ldfld, getPropertyField);
                        ilg.Emit(OpCodes.Callvirt, functionType.GetMethod("Invoke"));
                        ilg.Emit(OpCodes.Ret);

                        methodBuilder = typeBuilder.DefineMethod("set_" + property.Name, MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.Virtual, null, new Type[1] { property.PropertyType });
                        typeBuilder.DefineMethodOverride(methodBuilder, property.SetMethod);
                        ilg = methodBuilder.GetILGenerator();
                        ilg.Emit(OpCodes.Ldarg_0);
                        ilg.Emit(OpCodes.Ldfld, setPropertyField);
                        ilg.Emit(OpCodes.Ldarg_1);
                        ilg.Emit(OpCodes.Callvirt, actionType.GetMethod("Invoke", new Type[] { property.PropertyType }));
                        ilg.Emit(OpCodes.Nop);
                        ilg.Emit(OpCodes.Ret);
                    }

                    type = typeBuilder.CreateType();
                    generatedTypes.Add(typeName, type);
                }

                object obj = Activator.CreateInstance(type);

                foreach (var property in classProperties)
                {
                    Func<object> getFunction = () =>
                        {
                            variables.WaitInitalization();
                            return variables.GetType().GetProperty(property.Name).GetValue(variables);
                        };

                    Action<object> setAction = async (value) =>
                        {
                            variables.WaitInitalization();

                            string variableName = property.GetSingleAttribute<ServerVariableNameAttribute>().Name;

                            await Send(command, variableName, value);
                            variables.GetType().GetProperty(property.Name).SetValue(variables, value);
                        };

                    var tt = setAction.GetType();

                    var propertyTypeParameter = Expression.Parameter(property.PropertyType);
                    var setBody = Expression.Call(Expression.Constant(setAction), setAction.GetType().GetMethod("Invoke", new Type[] { typeof(object) }), Expression.Convert(propertyTypeParameter, typeof(object)));

                    var getBody = Expression.Convert(Expression.Call(Expression.Constant(getFunction), getFunction.GetType().GetMethod("Invoke")), property.PropertyType);
                    obj.GetType().GetField("__set_" + property.Name).SetValue(obj, Expression.Lambda(setBody, propertyTypeParameter).Compile());
                    obj.GetType().GetField("__get_" + property.Name).SetValue(obj, Expression.Lambda(getBody).Compile());
                }

                return (TAbstractClass)obj;
            }
        }
        #endregion

        public IFicsServerVariables ServerVariables { get; private set; }
        public IFicsServerInterfaceVariables ServerInterfaceVariables { get; private set; }
        public event GameStateChangeDelegate GameStateChange;

        #region Server lists
        public ServerList CensoredList { get; private set; }
        public ServerList WontPlayList { get; private set; }
        public ServerList ListeningChannelsList { get; private set; }
        private Dictionary<string, ServerList> serverLists = new Dictionary<string,ServerList>();

        private ServerList GetServerList(string name, bool isPublic)
        {
            ServerList list;

            if (!serverLists.TryGetValue(name, out list))
            {
                list = new ServerList(this);
                list.Name = name;
                list.Public = isPublic;
                serverLists.Add(name, list);
            }

            return list;
        }
        #endregion

        #region Commands
        public async Task<IFicsServerVariables> GetServerVariables(string username)
        {
            const string headerStart = "Variable settings of ";
            string output = await Execute(FicsCommand.ListServerVariables, username);
            string parsedUsername;
            var variables = ParseVariablesCommand(headerStart, output, out parsedUsername);

            if (parsedUsername != username)
            {
                throw new Exception("Unexpected username returned");
            }

            if (username == Username)
            {
                this.variables.Initialize(variables);
                return this.ServerVariables;
            }

            FicsServerVariables result = new FicsServerVariables();
            result.Initialize(variables);
            return result;
        }

        public async Task<IFicsServerInterfaceVariables> GetServerInterfaceVariables(string username)
        {
            const string headerStart = "Interface variable settings of ";
            string output = await Execute(FicsCommand.ListServerInterfaceVariables, username);
            string parsedUsername;
            var variables = ParseVariablesCommand(headerStart, output, out parsedUsername);

            if (parsedUsername != username)
            {
                throw new Exception("Unexpected username returned");
            }

            if (username == Username)
            {
                this.ivariables.Initialize(variables);
                return this.ServerInterfaceVariables;
            }

            var result = new FicsServerInterfaceVariables();
            result.Initialize(variables);
            return result;
        }

        public async Task<Game> ListGames(int gameNumber)
        {
            IList<Game> games = await ListGames(gameNumber.ToString());

            return games.Count > 0 ? games[0] : null;
        }

        public async Task<IList<Game>> ListGames(string playerUsernameStart = "")
        {
            string output = await Execute(FicsCommand.ListGames, playerUsernameStart);

            return ParseGames(output);
        }

        public async Task<IList<Game>> ListGames(GamesListingOptions options)
        {
            return await ListGames("/" + GenerateEnumString(options));
        }

        public async Task<BughouseListingResult> ListBughouse(BughouseListingOptions options = BughouseListingOptions.Games | BughouseListingOptions.Partnerships | BughouseListingOptions.Players)
        {
            string output = await Execute(FicsCommand.ListBughouse, GenerateEnumString(options));
            var result = new BughouseListingResult();
            StringReader reader = new StringReader(output);

            if (options.HasFlag(BughouseListingOptions.Games))
            {
                string line = reader.ReadLine();

                result.Games = new List<Game2x2>();
                Debug.Assert(line == "Bughouse games in progress");
                line = reader.ReadLine();
                while (!string.IsNullOrEmpty(line) && !line.StartsWith(" " + result.Games.Count + " game"))
                {
                    Game2x2 game2x2 = new Game2x2();

                    game2x2.First = ParseGame(line);
                    line = reader.ReadLine();
                    game2x2.Second = ParseGame(line);
                    result.Games.Add(game2x2);
                    line = reader.ReadLine();
                    Debug.Assert(line == "");
                    line = reader.ReadLine();
                }

                if (line == "")
                {
                    line = reader.ReadLine();
                }

                Debug.Assert(line.StartsWith(" " + result.Games.Count + " game"));
                line = reader.ReadLine();
                Debug.Assert(string.IsNullOrEmpty(line));
            }

            if (options.HasFlag(BughouseListingOptions.Partnerships))
            {
                string line = reader.ReadLine();

                result.Partnerships = new List<Partnership>();
                Debug.Assert(line == "Partnerships not playing bughouse");
                line = reader.ReadLine();
                while (!string.IsNullOrEmpty(line))
                {
                    Partnership partnership = new Partnership();
                    int position = 0;

                    partnership.Player1 = ParsePlayer(line, ref position, false);
                    SkipWhiteSpaces(line, ref position);
                    Debug.Assert(line[position] == '/');
                    position++;
                    partnership.Player2 = ParsePlayer(line, ref position, false);
                    result.Partnerships.Add(partnership);
                    line = reader.ReadLine();
                }

                line = reader.ReadLine();
                Debug.Assert(line.StartsWith(" " + result.Partnerships.Count + " partnership"));
                line = reader.ReadLine();
                Debug.Assert(string.IsNullOrEmpty(line));
            }

            if (options.HasFlag(BughouseListingOptions.Players))
            {
                string line = reader.ReadLine();

                Debug.Assert(line == "Unpartnered players with bugopen on");
                result.Players = ParsePlayers(reader, ServerVariables.ShowProvisionalRatings);
            }

            return result;
        }

        public async Task<IList<Player>> ListPlayers(PlayersListingOptions options = PlayersListingOptions.BlitzRating)
        {
            string output = await Execute(FicsCommand.ListPlayers, GenerateEnumString(options));
            StringReader reader = new StringReader(output);

            return ParsePlayers(reader, ServerVariables.ShowProvisionalRatings);
        }

        public async Task<IList<ServerList>> GetLists()
        {
            string output = await Execute(FicsCommand.ShowList);
            StringReader reader = new StringReader(output);
            string line = reader.ReadLine();
            List<ServerList> lists = new List<ServerList>();

            Debug.Assert(line == "Lists:");
            line = reader.ReadLine();
            Debug.Assert(string.IsNullOrEmpty(line));
            line = reader.ReadLine();
            while (!string.IsNullOrEmpty(line))
            {
                int listNameEnd = line.IndexOf(' ');
                string name = line.Substring(0, listNameEnd);
                bool isPublic = line.EndsWith("is PUBLIC");

                lists.Add(GetServerList(name, isPublic));
                line = reader.ReadLine();
            }

            return lists;
        }

        internal async Task<IList<string>> GetListEntries(string listName)
        {
            string output = await Execute(FicsCommand.ShowList, listName);
            StringReader reader = new StringReader(output);
            List<string> entries = new List<string>();
            string line = reader.ReadLine();

            Debug.Assert(line.StartsWith("--"));
            while (true)
            {
                line = reader.ReadLine();
                if (line == null)
                {
                    break;
                }

                string[] elements = line.Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

                foreach (string element in elements)
                {
                    entries.Add(element);
                }
            }

            return entries;
        }

        internal async void AddListEntry(string listName, string entry)
        {
            string output = await Execute(FicsCommand.AddToList, listName, entry);

            output = output.Trim();
            if (output != string.Format("[{0}] added to your {1} list.", entry, listName))
            {
                throw new Exception(output);
            }
        }

        internal async void RemoveListEntry(string listName, string entry)
        {
            string output = await Execute(FicsCommand.RemoveFromList, listName, entry);

            output = output.Trim();
            if (output != string.Format("[{0}] removed from your {1} list.", entry, listName))
            {
                throw new Exception(output);
            }
        }

        public async Task<ObserveGameResult> ObserveGame(Player player)
        {
            return await ObserveGame(player.Username);
        }

        public async Task<ObserveGameResult> ObserveGame(Game game)
        {
            return await ObserveGame(game.Id.ToString());
        }

        public async Task<ObserveGameResult> ObserveGame(string query)
        {
            ObserveGameResult result = new ObserveGameResult();
            string output = await Execute(FicsCommand.ObserveGame, query);
            StringReader reader = new StringReader(output);
            string line = reader.ReadLine();

            if (!line.StartsWith("You are now observing game "))
            {
                throw new Exception(query);
            }

            int gameIdEnd = line.IndexOf('.');
            int gameId = int.Parse(line.Substring(27, gameIdEnd - 27));

            // Parse game info
            string shortGameInfo = reader.ReadLine();
            string detailedGameInfo = null;

            Debug.Assert(shortGameInfo.StartsWith("Game " + gameId + ":"));
            line = reader.ReadLine();
            Debug.Assert(string.IsNullOrEmpty(line));

            if (ServerInterfaceVariables.DetailedGameInfo)
            {
                detailedGameInfo = reader.ReadLine();
                line = reader.ReadLine();
                Debug.Assert(string.IsNullOrEmpty(line));
            }

            // Take game info
            result.GameInfo = ParseGameInfo(shortGameInfo, detailedGameInfo, this.ServerVariables.ShowProvisionalRatings);

            // Parse game state (style 12)
            string style12gameLine = reader.ReadLine();
            string style12piecesLine = reader.ReadLine();

            result.GameState = ParseGameState(style12gameLine, style12piecesLine);
            return result;
        }
        #endregion

        private List<CommandState> executingCommands = new List<CommandState>();

        private async Task<CommandState> Send(FicsCommand command, params object[] args)
        {
            string commandName = "$$" + command.GetSingleAttribute<ServerCommandNameAttribute>().Name;
            CommandState commandState = null;

            if (ivariables.SendCommandsAsBlock)
            {
                int commandNumber = ReserveCommandNumber(command);

                commandState = executingCommands[commandNumber - 1];
                commandName = string.Format("{0} {1}", commandNumber, commandName);
            }

            StringBuilder commandText = new StringBuilder();

            commandText.Append(commandName);
            for (int i = 0; i < args.Length; i++)
            {
                commandText.Append(' ');
                commandText.Append(args[i].ToString());
            }

            await base.Send(commandText.ToString());
            return commandState;
        }

        private int ReserveCommandNumber(FicsCommand command)
        {
            lock (executingCommands)
            {
                int commandNumber = executingCommands.Count;

                for (int i = 0; i < executingCommands.Count; i++)
                {
                    if (!executingCommands[i].IsExecuting)
                    {
                        commandNumber = i;
                        break;
                    }
                }

                if (executingCommands.Count <= commandNumber)
                {
                    executingCommands.Add(new CommandState());
                }

                executingCommands[commandNumber].IsExecuting = true;
                executingCommands[commandNumber].Command = command;
                return commandNumber + 1;
            }
        }

        public override async Task Send(string message)
        {
            if (ivariables.SendCommandsAsBlock)
            {
                int commandNumber = ReserveCommandNumber(FicsCommand.NotSet);

                message = string.Format("{0} {1}", commandNumber, message);
            }

            await base.Send(message);
        }

        private async Task<string> Execute(FicsCommand command, params object[] args)
        {
            CommandState state = await Send(command, args);

            state.WaitForEnd();
            return state.Result;
        }

        protected override async Task LoginFinished()
        {
            await base.LoginFinished();
            await Send("iset block true");
            ivariables.SendCommandsAsBlock = true;
            var v1 = GetServerVariables(this.Username);
            var v2 = GetServerInterfaceVariables(this.Username);
        }

        protected override bool IsKnownMessage(ref string message)
        {
            if (message.Length > 0 && message[0] == CommandBlockStart)
            {
                // Parse all command result:
                // <BLOCK_START><command identifier><BLOCK_SEPARATOR><command code><BLOCK_SEPARATOR><command output><BLOCK_END>
                int messageEnd = message.IndexOf(CommandBlockEnd);
                Debug.Assert(messageEnd == message.Length - 1
                    || (messageEnd == message.Length - 2 && message[message.Length - 1] == '\n'));
                int firstBlockSeparator = message.IndexOf(CommandBlockSeparator);
                int secondBlockSeparator = message.IndexOf(CommandBlockSeparator, firstBlockSeparator + 1);
                int commandNumber = int.Parse(message.Substring(1, firstBlockSeparator - 1));
                int commandCode = int.Parse(message.Substring(firstBlockSeparator + 1, secondBlockSeparator - firstBlockSeparator - 1));
                string commandResult = message.Substring(secondBlockSeparator + 1, messageEnd - secondBlockSeparator - 1);

                CommandState commandState = executingCommands[commandNumber - 1];

                if (commandState.Command == FicsCommand.NotSet)
                {
                    message = commandCode + "\n" + commandResult;
                    commandState.IsExecuting = false;
                    return false;
                }

                if (commandCode != (int)commandState.CommandCode)
                {
                    throw new Exception("Unexpected command code " + commandCode);
                }

                commandState.Result = commandResult;
                commandState.IsExecuting = false;
                return true;
            }

            // Check if it is game state change message (from observing a game)
            if (message.StartsWith("\n<12>"))
            {
                if (GameStateChange != null)
                {
                    StringReader reader = new StringReader(message);
                    string emptyLine = reader.ReadLine();
                    string gameLine = reader.ReadLine();
                    string piecesLine = reader.ReadLine();

                    Debug.Assert(string.IsNullOrEmpty(emptyLine));

                    GameState gameState = ParseGameState(gameLine, piecesLine);

                    GameStateChange(gameState);
                }

                return true;
            }
            else if (message.StartsWith("\n<b1>"))
            {
                if (GameStateChange != null)
                {
                    StringReader reader = new StringReader(message);
                    string emptyLine = reader.ReadLine();
                    string piecesLine = reader.ReadLine();

                    Debug.Assert(string.IsNullOrEmpty(emptyLine));

                    GameState gameState = ParseGameState(null, piecesLine);

                    GameStateChange(gameState);
                }

                return true;
            }

            return false;
        }

        #region Parsing
        private static IList<Game> ParseGames(string output)
        {
            List<Game> games = new List<Game>();
            StringReader reader = new StringReader(output);
            string line = reader.ReadLine();

            Debug.Assert(string.IsNullOrEmpty(line));
            line = reader.ReadLine();
            while (!string.IsNullOrEmpty(line))
            {
                games.Add(ParseGame(line));
                line = reader.ReadLine();
            }

            Debug.Assert(string.IsNullOrEmpty(line));
            line = reader.ReadLine();
            Debug.Assert(line.StartsWith("  " + (games.Count - games.Count(g => g.Type == GameType.Bughouse) / 2) + " game"));
            line = reader.ReadLine();
            Debug.Assert(string.IsNullOrEmpty(line));

            return games;
        }

        private static Game ParseGame(string line)
        {
            Game game = new Game();
            int position = 0;

            // Game Id
            SkipWhiteSpaces(line, ref position);
            int nextSpaceIndex = line.IndexOf(' ', position);
            game.Id = int.Parse(line.Substring(position, nextSpaceIndex - position));
            position = nextSpaceIndex + 1;

            // Check if game is examined or setup
            if (line[position] == '(')
            {
                position++;
                if (line[position] == 'E')
                {
                    Debug.Assert(line.Substring(position, 5) == "Exam.");
                    position += 5;
                    game.Examined = true;
                }
                else if (line[position] == 'S')
                {
                    Debug.Assert(line.Substring(position, 5) == "Setup");
                    position += 5;
                    game.InSetup = true;
                }
                else
                {
                    throw new Exception("Unexpected game type");
                }

                // Players
                game.WhitePlayer = ParsePlayer(line, ref position, false);
                game.BlackPlayer = ParsePlayer(line, ref position, false);
                SkipWhiteSpaces(line, ref position);
                Debug.Assert(line[position] == ')');
            }
            else
            {
                // Players
                game.WhitePlayer = ParsePlayer(line, ref position, false);
                game.BlackPlayer = ParsePlayer(line, ref position, false);
            }

            // Game type
            position = line.IndexOf('[') + 1;
            Debug.Assert(position != 0);
            game.Private = line[position++] == 'p';
            game.Type = ParseEnum<GameType>(line.Substring(position++, 1));
            game.Rated = line[position++] == 'r';

            // Clock start
            SkipWhiteSpaces(line, ref position);
            nextSpaceIndex = line.IndexOf(' ', position);
            game.ClockStart = TimeSpan.FromMinutes(int.Parse(line.Substring(position, nextSpaceIndex - position)));
            position = nextSpaceIndex + 1;

            // Time increment
            SkipWhiteSpaces(line, ref position);
            nextSpaceIndex = line.IndexOf(']', position);
            game.TimeIncrement = TimeSpan.FromSeconds(int.Parse(line.Substring(position, nextSpaceIndex - position)));
            position = nextSpaceIndex + 1;

            if (!game.Examined && !game.InSetup)
            {
                // White clock
                SkipWhiteSpaces(line, ref position);
                nextSpaceIndex = line.IndexOf('-', position);
                game.WhiteClock = ParseTime(line.Substring(position, nextSpaceIndex - position).Trim());
                position = nextSpaceIndex + 1;

                // Black clock
                nextSpaceIndex = line.IndexOf('(', position);
                game.WhiteClock = ParseTime(line.Substring(position, nextSpaceIndex - position).Trim());
                position = nextSpaceIndex;

                // White strength
                Debug.Assert(line[position] == '(');
                position++;
                nextSpaceIndex = line.IndexOf('-', position);
                game.WhiteStrength = int.Parse(line.Substring(position, nextSpaceIndex - position));
                position = nextSpaceIndex + 1;

                // Black strength
                nextSpaceIndex = line.IndexOf(')', position);
                game.BlackStrength = int.Parse(line.Substring(position, nextSpaceIndex - position));
                position = nextSpaceIndex;
                Debug.Assert(line[position] == ')');
                position++;
            }

            // Next on move
            SkipWhiteSpaces(line, ref position);
            game.WhiteMove = line[position++] == 'W';
            Debug.Assert(line[position] == ':');
            position++;

            // Move number
            game.Move = int.Parse(line.Substring(position).Trim());
            return game;
        }

        private static TEnum ParseEnum<TEnum>(string value) where TEnum : Enum
        {
            foreach (TEnum enumValue in Enum.GetValues(typeof(TEnum)))
            {
                if (typeof(TEnum).GetSingleAttribute<ServerVariableNameAttribute>(enumValue.ToString()).Name == value
                    || enumValue.ToString().Equals(value, StringComparison.CurrentCultureIgnoreCase))
                {
                    return enumValue;
                }
            }

            throw new Exception("Unable to parse enum " + typeof(TEnum));
        }

        private static string GenerateEnumString<TEnum>(TEnum options) where TEnum : struct
        {
            string result = "";

            foreach (TEnum option in Enum.GetValues(typeof(TEnum)))
            {
                if (((int)(object)options & (int)(object)option) == (int)(object)option)
                {
                    result += typeof(TEnum).GetSingleAttribute<ServerVariableNameAttribute>(option.ToString()).Name;
                }
            }

            return result;
        }

        private static TimeSpan ParseTime(string str)
        {
            return TimeSpan.ParseExact(str, new string[] { @"%h\:mm\:ss", @"%m\:ss", @"%h\:mm\:ss\.fff", @"%m\:ss\.fff" }, null);
        }

        private static void SkipWhiteSpaces(string line, ref int position)
        {
            while (position < line.Length && char.IsWhiteSpace(line[position]))
            {
                position++;
            }
        }

        private static IList<Player> ParsePlayers(TextReader reader, bool hasProvisionalRating)
        {
            var players = new List<Player>();
            string line = reader.ReadLine();

            Debug.Assert(string.IsNullOrEmpty(line));
            line = reader.ReadLine();
            while (!string.IsNullOrEmpty(line))
            {
                int position = 0;

                SkipWhiteSpaces(line, ref position);
                while (position < line.Length)
                {
                    players.Add(ParsePlayer(line, ref position, hasProvisionalRating));
                    SkipWhiteSpaces(line, ref position);
                }

                line = reader.ReadLine();
            }

            line = reader.ReadLine();
            Debug.Assert(line.StartsWith(" " + players.Count + " player"));
            line = reader.ReadLine();
            Debug.Assert(string.IsNullOrEmpty(line));
            return players;
        }

        private static Player ParsePlayer(string line, ref int position, bool hasProvisionalRating)
        {
            Player player = new Player();

            // Parse rating
            PlayerProvisionalRating provisionalRating = PlayerProvisionalRating.Unknown;

            player.Rating = ParseRating(line, ref position, hasProvisionalRating, ref provisionalRating);
            player.ProvisionalRating = provisionalRating;

            // Status
            player.Status = ParseEnum<PlayerStatus>(line.Substring(position++, 1));

            // Username
            SkipWhiteSpaces(line, ref position);
            int usernameEnd = position;
            while (usernameEnd < line.Length && (char.IsLetter(line[usernameEnd]) || line[usernameEnd] == '.' || line[usernameEnd] == ',') || char.IsDigit(line[usernameEnd]))
            {
                usernameEnd++;
            }

            player.Username = line.Substring(position, usernameEnd - position);
            position = usernameEnd;

            // Parse account status
            bool found = true;

            while (found && position < line.Length)
            {
                found = false;
                if (line[position] == '(')
                {
                    foreach (AccountStatus status in Enum.GetValues(typeof(AccountStatus)))
                    {
                        string statusString = "(" + status.GetSingleAttribute<ServerVariableNameAttribute>().Name + ")";

                        if (position + statusString.Length <= line.Length && line.Substring(position, statusString.Length) == statusString)
                        {
                            player.AccountStatus |= status;
                            position += statusString.Length;
                            found = true;
                            break;
                        }
                    }
                }
            }

            return player;
        }

        private static Dictionary<string, string> ParseVariablesCommand(string commandHeader, string command, out string username)
        {
            // Check command header
            if (!command.StartsWith(commandHeader))
            {
                username = "";
                return null;
            }

            StringReader reader = new StringReader(command);
            string line = reader.ReadLine();

            username = line.Substring(commandHeader.Length);
            if (username.EndsWith(":"))
            {
                username = username.Substring(0, username.Length - 1);
            }

            return ParseVariables(reader);
        }

        private static Dictionary<string, string> ParseVariables(TextReader reader)
        {
            Dictionary<string, string> variables = new Dictionary<string, string>();

            while (true)
            {
                string line = reader.ReadLine();

                if (line == null)
                {
                    break;
                }

                if (line.Contains(':'))
                {
                    string variable = line.Substring(0, line.IndexOf(':'));
                    string value = line.Substring(variable.Length + 2);

                    variables.Add(variable, value);
                }
                else
                {
                    string[] vars = line.Split("\t ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

                    foreach (string var in vars)
                    {
                        string[] ss = var.Split("=".ToCharArray());

                        if (ss.Length == 2)
                        {
                            variables.Add(ss[0], ss[1]);
                        }
                    }
                }
            }

            return variables;
        }

        private static GameInfo ParseGameInfo(string line, string detailedGameInfo, bool hasProvisionalRating)
        {
            // Since rating is being printed inside brackets with witdh=4, there is possibility of having spaces after (
            line = line.Replace("( ", "(").Replace("( ", "(").Replace("( ", "(");

            // Parse simple info (line)
            GameInfo info = new GameInfo();
            string[] tokens = line.Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            PlayerProvisionalRating whiteProvisitionalRating = PlayerProvisionalRating.Unknown;
            PlayerProvisionalRating blackProvisitionalRating = PlayerProvisionalRating.Unknown;

            Debug.Assert(tokens[0] == "Game");
            Debug.Assert(tokens[1].EndsWith(":"));
            info.GameId = int.Parse(tokens[1].Substring(0, tokens[1].Length - 1));
            info.WhitePlayer = new Player();
            info.WhitePlayer.Username = tokens[2];
            Debug.Assert(tokens[3].StartsWith("(") && tokens[3].EndsWith(")"));
            info.WhitePlayer.Rating = ParseRating(tokens[3].Substring(1, tokens[3].Length - 2), hasProvisionalRating, ref whiteProvisitionalRating);
            info.WhitePlayer.ProvisionalRating = whiteProvisitionalRating;
            info.BlackPlayer = new Player();
            info.BlackPlayer.Username = tokens[4];
            Debug.Assert(tokens[5].StartsWith("(") && tokens[5].EndsWith(")"));
            info.BlackPlayer.Rating = ParseRating(tokens[5].Substring(1, tokens[5].Length - 2), hasProvisionalRating, ref blackProvisitionalRating);
            info.BlackPlayer.ProvisionalRating = blackProvisitionalRating;
            info.Rated = tokens[6] == "rated";
            info.Type = ParseEnum<GameType>(tokens[7]);
            info.ClockStart = TimeSpan.FromMinutes(int.Parse(tokens[8]));
            info.TimeIncrement = TimeSpan.FromSeconds(int.Parse(tokens[9]));

            // Parse detailed game info
            if (detailedGameInfo != null)
            {
                tokens = detailedGameInfo.Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                Debug.Assert(tokens[0] == "<g1>");

                // Game Id
                Debug.Assert(info.GameId == int.Parse(tokens[1]));

                // Other fields
                Dictionary<string, string> variables = new Dictionary<string, string>();

                for (int i = 2; i < tokens.Length; i++)
                {
                    string[] t = tokens[i].Split("=".ToCharArray());

                    Debug.Assert(t.Length == 2);
                    variables.Add(t[0], t[1]);
                }

                string[] registered = variables["u"].Split(",".ToCharArray());
                string[] initialTime = variables["it"].Split(",".ToCharArray());
                string[] increment = variables["i"].Split(",".ToCharArray());
                string[] ratings = variables["rt"].Split(",".ToCharArray());
                string[] timeSeal = variables["ts"].Split(",".ToCharArray());

                //Debug.Assert(info.Type == ParseEnum<GameType>(variables["t"]));
                //Debug.Assert(info.WhitePlayer.IsRegistered == (registered[0] == "1"));
                //Debug.Assert(info.BlackPlayer.IsRegistered == (registered[1] == "1"));
                //info.WhiteClockStart = TimeSpan.FromSeconds(int.Parse(initialTime[0]));
                //info.BlackClockStart = TimeSpan.FromSeconds(int.Parse(initialTime[1]));
                //info.WhiteTimeIncrement = TimeSpan.FromSeconds(int.Parse(increment[0]));
                //info.BlackTimeIncrement = TimeSpan.FromSeconds(int.Parse(increment[1]));
                //whiteProvisitionalRating = PlayerProvisionalRating.Unknown;
                //Debug.Assert(info.WhitePlayer.Rating == ParseRating(ratings[0], hasProvisionalRating, ref whiteProvisitionalRating));
                //Debug.Assert(info.WhitePlayer.ProvisionalRating == whiteProvisitionalRating);
                //blackProvisitionalRating = PlayerProvisionalRating.Unknown;
                //Debug.Assert(info.BlackPlayer.Rating == ParseRating(ratings[1], hasProvisionalRating, ref blackProvisitionalRating));
                //Debug.Assert(info.BlackPlayer.ProvisionalRating == blackProvisitionalRating);
                //Debug.Assert(info.Rated == (variables["r"] == "1"));

                info.Private = variables["p"] == "1";
                info.PartnersGameId = int.Parse(variables["pt"]);

                info.WhiteUsesTimeSeal = timeSeal[0] == "1";
                info.BlackUsesTimeSeal = timeSeal[1] == "1";
            }

            return info;
        }

        private static int ParseRating(string line, bool hasProvisionalRating, ref PlayerProvisionalRating provisionalRating)
        {
            int position = 0;

            return ParseRating(line, ref position, hasProvisionalRating, ref provisionalRating);
        }

        private static int ParseRating(string line, bool hasProvisionalRating)
        {
            int position = 0;

            return ParseRating(line, ref position, hasProvisionalRating);
        }

        private static int ParseRating(string line, ref int position, bool hasProvisionalRating)
        {
            PlayerProvisionalRating provisionalRating = PlayerProvisionalRating.Unknown;

            return ParseRating(line, ref position, hasProvisionalRating, ref provisionalRating);
        }

        private static int ParseRating(string line, ref int position, bool hasProvisionalRating, ref PlayerProvisionalRating provisionalRating)
        {
            // Rating
            int rating;

            SkipWhiteSpaces(line, ref position);
            if (line[position] == '-')
            {
                Debug.Assert(line.Substring(position, 4) == "----");
                position += 4;
                rating = -1;
            }
            else if (line[position] == '+')
            {
                Debug.Assert(line.Substring(position, 4) == "++++");
                position += 4;
                rating = -2;
            }
            else
            {
                int ratingEnd = position;
                while (ratingEnd < line.Length && char.IsDigit(line[ratingEnd]))
                {
                    ratingEnd++;
                }

                rating = int.Parse(line.Substring(position, ratingEnd - position));
                position = ratingEnd;
            }

            // Provisional rating
            if (hasProvisionalRating && position < line.Length)
            {
                provisionalRating = ParseEnum<PlayerProvisionalRating>(line.Substring(position++, 1));
            }

            return rating;
        }

        private static GameState ParseGameState(string gameLine, string piecesLine)
        {
            GameState state = new GameState();

            if (!string.IsNullOrEmpty(gameLine))
            {
                string[] tokens = gameLine.Split(" ".ToCharArray());

                Debug.Assert(tokens[0] == "<12>");

                state.Board = new ChessPieceWithColor[8, 8];
                for (int y = 0; y < 8; y++)
                {
                    Debug.Assert(tokens[y + 1].Length == 8);
                    for (int x = 0; x < 8; x++)
                    {
                        char piece = tokens[y + 1][x];

                        if (piece != '-')
                        {
                            state.Board[y, x] = new ChessPieceWithColor()
                            {
                                Piece = ParseEnum<ChessPiece>(char.ToUpper(piece).ToString()),
                                Color = char.IsUpper(piece) ? ChessPieceColor.White : ChessPieceColor.Black,
                            };
                        }
                    }
                }

                state.WhiteMove = tokens[9] == "W";
                state.DoublePushPawnColumn = int.Parse(tokens[10]);
                state.WhiteCanCastleShort = tokens[11] == "1";
                state.WhiteCanCastleLong = tokens[12] == "1";
                state.BlackCanCastleShort = tokens[13] == "1";
                state.BlackCanCastleLong = tokens[14] == "1";
                state.MovesSinceLastIrreversibleMove = int.Parse(tokens[15]);
                state.GameId = int.Parse(tokens[16]);
                state.WhitePlayerUsername = tokens[17];
                state.BlackPlayerUsername = tokens[18];
                state.ClockStart = TimeSpan.FromMinutes(int.Parse(tokens[20]));
                state.TimeIncrement = TimeSpan.FromSeconds(int.Parse(tokens[21]));
                state.WhiteClock = TimeSpan.FromMilliseconds(double.Parse(tokens[24]));
                state.BlackClock = TimeSpan.FromMilliseconds(double.Parse(tokens[25]));
                state.Move = int.Parse(tokens[26]);
                state.LastMoveVerbose = tokens[27];
                Debug.Assert(tokens[28].StartsWith("(") && tokens[28].EndsWith(")"));
                state.LastMoveTime = ParseTime(tokens[28].Substring(1, tokens[28].Length - 2));
                state.LastMove = tokens[29];
                state.BlackAtBottom = tokens[30] == "1";

                // TODO: Parse
                //* my relation to this game:
                //    -3 isolated position, such as for "ref 3" or the "sposition" command
                //    -2 I am observing game being examined
                //     2 I am the examiner of this game
                //    -1 I am playing, it is my opponent's move
                //     1 I am playing and it is my move
                //     0 I am observing a game being played
                //int whiteMaterialStrength = int.Parse(tokens[22]);
                //int blackMaterialStrength = int.Parse(tokens[23]);
            }

            if (!string.IsNullOrEmpty(piecesLine))
            {
                string[] tokens = piecesLine.Split(" ".ToCharArray());

                Debug.Assert(tokens[0] == "<b1>");
                Debug.Assert(tokens[1] == "game");
                Debug.Assert(tokens[3] == "white");
                Debug.Assert(tokens[5] == "black");
                Debug.Assert(tokens.Length == 7 || tokens[7] == "<-");

                int gameId = int.Parse(tokens[2]);
                Debug.Assert(state.GameId == 0 || state.GameId == gameId);
                state.GameId = gameId;
                Debug.Assert(tokens[4].StartsWith("[") && tokens[4].EndsWith("]"));
                Debug.Assert(tokens[6].StartsWith("[") && tokens[6].EndsWith("]"));
                string whitePieces = tokens[4].Substring(1, tokens[4].Length - 2);
                string blackPieces = tokens[6].Substring(1, tokens[6].Length - 2);

                state.WhitePieces = new List<ChessPiece>();
                foreach (char piece in whitePieces)
                {
                    state.WhitePieces.Add(ParseEnum<ChessPiece>(char.ToUpper(piece).ToString()));
                }

                state.BlackPieces = new List<ChessPiece>();
                foreach (char piece in blackPieces)
                {
                    state.BlackPieces.Add(ParseEnum<ChessPiece>(char.ToUpper(piece).ToString()));
                }
            }

            return state;
        }
        #endregion
    }
}
