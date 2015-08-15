namespace Internet.Chess.Server.Fics
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;

    /// <summary>
    /// Free Internet Chess Server client
    /// </summary>
    public class FicsClient : IcsClient
    {
        #region Event delegates
        /// <summary>
        /// Delegate for game state changes (while observing the game).
        /// </summary>
        /// <param name="gameState">State of the game.</param>
        public delegate void GameStateChangeDelegate(GameState gameState);

        /// <summary>
        /// Delegate for game ended (while observing the game)
        /// </summary>
        /// <param name="gameId">The game identifier.</param>
        public delegate void GameEndedDelegate(GameEndedInfo info);

        /// <summary>
        /// Delegate for game stopped observing.
        /// </summary>
        /// <param name="gameId">The game identifier.</param>
        public delegate void GameStoppedObservingDelegate(int gameId);

        /// <summary>
        /// Delegate when new message arrives.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="message">The message.</param>
        public delegate void MessageReceivedDelegate(string username, string message);

        /// <summary>
        /// Delegate when new message arrives over the channel.
        /// </summary>
        /// <param name="channel">The channel.</param>
        /// <param name="username">The username.</param>
        /// <param name="message">The message.</param>
        public delegate void ChannelMessageReceivedDelegate(int channel, string username, string message);

        /// <summary>
        /// Delegate when new announcement message arrives.
        /// </summary>
        /// <param name="announcement">The announcement message.</param>
        public delegate void AnnouncementDelegate(string announcement);

        /// <summary>
        /// Delegate when new whisper/kibitz message arrives.
        /// </summary>
        /// <param name="player">The player.</param>
        /// <param name="gameId">The game identifier.</param>
        /// <param name="message">The message.</param>
        public delegate void InGameMessageDelegate(Player player, int gameId, string message);

        /// <summary>
        /// Delegate when new seeking message arrives.
        /// </summary>
        /// <param name="seekInfo">The seek information.</param>
        public delegate void SeekingDelegate(SeekInfo seekInfo);

        /// <summary>
        /// Delegate when followed player started a new game.
        /// </summary>
        /// <param name="game">The game.</param>
        public delegate void FollowedPlayerStartedGameDelegate(ObserveGameResult game);
        #endregion

        #region Constants
        public const string DefaultPrompt = "fics% ";
        public const string DefaultServer = "freechess.org";
        public const int DefaultServerPort = 5000;
        public const int AlternateServerPort = 23;
        public const string DefaultNewLine = "\n\r";

        private const char CommandBlockStart = (char)21;
        private const char CommandBlockSeparator = (char)22;
        private const char CommandBlockEnd = (char)23;
        private const char CommandBlockPoseStart = (char)24;
        private const char CommandBlockPoseEnd = (char)25;
        #endregion

        #region Cached values of server variables
        /// <summary>
        /// The server variables cached values
        /// </summary>
        internal FicsServerVariables variables = new FicsServerVariables();

        /// <summary>
        /// The server interface cached variables
        /// </summary>
        internal FicsServerInterfaceVariables ivariables = new FicsServerInterfaceVariables();
        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="FicsClient"/> class.
        /// </summary>
        /// <param name="server">The server address.</param>
        /// <param name="port">The server port.</param>
        /// <param name="prompt">The server prompt.</param>
        /// <param name="newLine">The server new line.</param>
        public FicsClient(string server = DefaultServer, int port = DefaultServerPort, string prompt = DefaultPrompt, string newLine = DefaultNewLine)
            : base(server, port, prompt, newLine)
        {
            ServerVariables = new AutoFicsServerVariables(this, variables, FicsCommand.SetServerVariable);
            ServerInterfaceVariables = new AutoFicsServerInterfaceVariables(this, ivariables, FicsCommand.SetServerInterfaceVariable);

            // Create standard server personal lists
            CensoredList = GetServerList(ServerList.CensoredList, isPublic: false);
            WontPlayList = GetServerList(ServerList.WontPlayList, isPublic: false);
            ListeningChannelsList = GetServerList(ServerList.ListeningChannelsList, isPublic: false);
        }

        /// <summary>
        /// Gets the server variables.
        /// </summary>
        public IFicsServerVariables ServerVariables { get; private set; }

        /// <summary>
        /// Gets the server interface variables.
        /// </summary>
        public IFicsServerInterfaceVariables ServerInterfaceVariables { get; private set; }

        #region Events
        /// <summary>
        /// Occurs when game state changes (while observing the game).
        /// </summary>
        public event GameStateChangeDelegate GameStateChange;

        /// <summary>
        /// Occurs when game has ended.
        /// </summary>
        public event GameEndedDelegate GameEnded;

        /// <summary>
        /// Occurs when game has ended and removed from observing list.
        /// </summary>
        public event GameStoppedObservingDelegate GameStoppedObserving;

        /// <summary>
        /// Occurs when new message is received.
        /// </summary>
        public event MessageReceivedDelegate MessageReceived;

        /// <summary>
        /// Occurs when new shout message is received.
        /// </summary>
        public event MessageReceivedDelegate ShoutMessageReceived;

        /// <summary>
        /// Occurs when new chess shout message is received.
        /// </summary>
        public event MessageReceivedDelegate ChessShoutMessageReceived;

        /// <summary>
        /// Occurs when new channel message is received.
        /// </summary>
        public event ChannelMessageReceivedDelegate ChannelMessageReceived;

        /// <summary>
        /// Occurs when new announcement message is received.
        /// </summary>
        public event AnnouncementDelegate Announcement;

        /// <summary>
        /// Occurs when new whisper message arrives.
        /// </summary>
        public event InGameMessageDelegate Whisper;

        /// <summary>
        /// Occurs when new kibitz message arrives.
        /// </summary>
        public event InGameMessageDelegate Kibitz;

        /// <summary>
        /// Occurs when new seeking message arrives.
        /// </summary>
        public event SeekingDelegate Seeking;

        /// <summary>
        /// Occurs when player, whom you are following, started a new game.
        /// </summary>
        public event FollowedPlayerStartedGameDelegate FollowedPlayerStartedGame;
        #endregion

        #region Server lists
        /// <summary>
        /// Gets the censored list.
        /// When a player is on your censor list, you will not hear anything from
        /// him/her when he/she uses a tell, shout, cshout, match, kibitz, whisper,
        /// say or message. Tells to you as individual and tells to a channel are both
        /// affected. Since match commands are also filtered, players on your censor
        /// list cannot challenge you to a game of chess. Lastly, messages from a player
        /// on your censor list will also be rejected.
        /// 
        /// When users on your censor list send you a direct message using "tell", or
        /// an indirect message using "message", they will be notified of their being
        /// on your censor list. Your censor list is otherwise private and cannot be
        /// read by other users.
        /// </summary>
        public ServerList CensoredList { get; private set; }

        /// <summary>
        /// Gets the wont play list.
        /// When a player is on your "won't play" list, all match requests from that player
        /// will be declined automatically.
        /// </summary>
        public ServerList WontPlayList { get; private set; }

        /// <summary>
        /// Gets the listening channels list.
        /// When a channel is on your list, you will receive messages(tells) sent to that
        /// channel. Also, if you send a tell to channels 1, 2, 49 or 50, the FICS will
        /// attempt to add it to your channel list (unless it is already there).
        /// </summary>
        public ServerList ListeningChannelsList { get; private set; }

        /// <summary>
        /// The cached server list objects used when querying server for the list.
        /// </summary>
        private Dictionary<string, ServerList> serverLists = new Dictionary<string,ServerList>();

        /// <summary>
        /// Gets the server list from cached server list objects. If list is not in the cached list,
        /// it will be automatically added.
        /// </summary>
        /// <param name="name">The name of the list.</param>
        /// <param name="isPublic">if set to <c>true</c> the list is public.</param>
        /// <returns>Cached server list object.</returns>
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
        /// <summary>
        /// Gets the server variables for a given user.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <returns>Server variables</returns>
        public async Task<IFicsServerVariables> GetServerVariables(string username)
        {
            const string headerStart = "Variable settings of ";
            string output = await Execute(FicsCommand.ListServerVariables, username);

            try
            {
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
            catch (Exception ex)
            {
                throw new AggregateException("Parsing exception. Command:\n'" + output + "'\n", ex);
            }
        }

        /// <summary>
        /// Gets the server interface variables.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <returns>Server interface variables</returns>
        public async Task<IFicsServerInterfaceVariables> GetServerInterfaceVariables(string username)
        {
            const string headerStart = "Interface variable settings of ";
            string output = await Execute(FicsCommand.ListServerInterfaceVariables, username);

            try
            {
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
            catch (Exception ex)
            {
                throw new AggregateException("Parsing exception. Command:\n'" + output + "'\n", ex);
            }
        }

        /// <summary>
        /// Gets the game info from the server.
        /// </summary>
        /// <param name="gameNumber">The game number.</param>
        /// <returns>The game</returns>
        public async Task<Game> GetGame(int gameNumber)
        {
            IList<Game> games = await ListGames(gameNumber.ToString());

            return games.FirstOrDefault(g => g.Id == gameNumber);
        }

        /// <summary>
        /// Lists the games from a given user.
        /// </summary>
        /// <param name="playerUsernameStart">The player username start.</param>
        /// <returns>List of games</returns>
        public async Task<IList<Game>> ListGames(string playerUsernameStart = "")
        {
            string output = await Execute(FicsCommand.ListGames, playerUsernameStart);

            try
            {
                if (!output.StartsWith("\n"))
                    throw new Exception(output);
                return ParseGames(output);
            }
            catch (Exception ex)
            {
                throw new AggregateException("Parsing exception. Command:\n'" + output + "'\n", ex);
            }
        }

        /// <summary>
        /// Lists the games currently in progress on the server.
        /// </summary>
        /// <param name="options">The listing options.</param>
        /// <returns>List of games</returns>
        public async Task<IList<Game>> ListGames(GamesListingOptions options)
        {
            return await ListGames("/" + GenerateEnumString(options));
        }

        /// <summary>
        /// Lists the bughouse current games/partnerships/available partners.
        /// </summary>
        /// <param name="options">The listing options.</param>
        /// <returns>Games/partnerships/available partners</returns>
        public async Task<BughouseListingResult> ListBughouse(BughouseListingOptions options = BughouseListingOptions.Games | BughouseListingOptions.Partnerships | BughouseListingOptions.Players)
        {
            string output = await Execute(FicsCommand.ListBughouse, GenerateEnumString(options));

            try
            {
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
                        result.Partnerships.Add(ParsePartnership(line, ServerVariables.ShowProvisionalRatings));
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
            catch (Exception ex)
            {
                throw new AggregateException("Parsing exception. Command:\n'" + output + "'\n", ex);
            }
        }

        /// <summary>
        /// Lists the players currently active on the server.
        /// </summary>
        /// <param name="options">The listing options.</param>
        /// <returns>List of players</returns>
        public async Task<IList<Player>> ListPlayers(PlayersListingOptions options = PlayersListingOptions.BlitzRating)
        {
            string output = await Execute(FicsCommand.ListPlayers, GenerateEnumString(options));

            try
            {
                StringReader reader = new StringReader(output);

                return ParsePlayers(reader, ServerVariables.ShowProvisionalRatings);
            }
            catch (Exception ex)
            {
                throw new AggregateException("Parsing exception. Command:\n'" + output + "'\n", ex);
            }
        }

        /// <summary>
        /// Gets the server lists.
        /// </summary>
        /// <returns>All server lists</returns>
        public async Task<IList<ServerList>> GetLists()
        {
            string output = await Execute(FicsCommand.ShowList);

            try
            {
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
            catch (Exception ex)
            {
                throw new AggregateException("Parsing exception. Command:\n'" + output + "'\n", ex);
            }
        }

        /// <summary>
        /// Gets the server list entries.
        /// </summary>
        /// <param name="listName">Name of the list.</param>
        /// <returns>Server list entries</returns>
        internal async Task<IList<string>> GetListEntries(string listName)
        {
            string output = await Execute(FicsCommand.ShowList, listName);

            try
            {
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
            catch (Exception ex)
            {
                throw new AggregateException("Parsing exception. Command:\n'" + output + "'\n", ex);
            }
        }

        /// <summary>
        /// Adds the server list entry.
        /// </summary>
        /// <param name="listName">Name of the list.</param>
        /// <param name="entry">The entry.</param>
        internal async void AddListEntry(string listName, string entry)
        {
            string output = await Execute(FicsCommand.AddToList, listName, entry);

            output = output.Trim();
            if (output != string.Format("[{0}] added to your {1} list.", entry, listName))
            {
                throw new Exception(output);
            }
        }

        /// <summary>
        /// Removes the list entry.
        /// </summary>
        /// <param name="listName">Name of the list.</param>
        /// <param name="entry">The entry.</param>
        internal async void RemoveListEntry(string listName, string entry)
        {
            string output = await Execute(FicsCommand.RemoveFromList, listName, entry);

            output = output.Trim();
            if (output != string.Format("[{0}] removed from your {1} list.", entry, listName))
            {
                throw new Exception(output);
            }
        }

        /// <summary>
        /// Starts observing the player's game.
        /// </summary>
        /// <param name="player">The player.</param>
        /// <returns>Game info and game state</returns>
        public async Task<ObserveGameResult> StartObservingGame(Player player)
        {
            return await StartObservingGame(player.Username);
        }

        /// <summary>
        /// Starts observing the game.
        /// </summary>
        /// <param name="game">The game.</param>
        /// <returns>Game info and game state</returns>
        public async Task<ObserveGameResult> StartObservingGame(Game game)
        {
            return await StartObservingGame(game.Id.ToString());
        }

        /// <summary>
        /// Starts observing the game.
        /// </summary>
        /// <param name="gameId">The game id.</param>
        /// <returns>Game info and game state</returns>
        public async Task<ObserveGameResult> StartObservingGame(int gameId)
        {
            return await StartObservingGame(gameId.ToString());
        }

        /// <summary>
        /// Starts observing the game.
        /// </summary>
        /// <param name="query">The query (username/game id).</param>
        /// <returns>Game info and game state</returns>
        public async Task<ObserveGameResult> StartObservingGame(string query)
        {
            string output = await Execute(FicsCommand.ObserveGame, query);

            try
            {
                StringReader reader = new StringReader(output);

                return ParseObserveGame(reader, ServerInterfaceVariables.DetailedGameInfo, ServerVariables.ShowProvisionalRatings);
            }
            catch (Exception ex)
            {
                throw new AggregateException("Parsing exception. Command:\n'" + output + "'\n", ex);
            }
        }

        /// <summary>
        /// Gets the move list.
        /// </summary>
        /// <param name="game">The game.</param>
        /// <returns>Move list</returns>
        public async Task<ChessMoveList> GetMoveList(Game game)
        {
            return await GetMoveList(game.Id);
        }

        /// <summary>
        /// Gets the move list.
        /// </summary>
        /// <param name="gameId">The game id.</param>
        /// <returns>Move list</returns>
        public async Task<ChessMoveList> GetMoveList(int gameId)
        {
            string output = await Execute(FicsCommand.MoveList, gameId);

            try
            {
                if (!output.StartsWith("\nMovelist for game "))
                {
                    throw new Exception(output);
                }

                return ParseMoveList(output);
            }
            catch (Exception ex)
            {
                throw new AggregateException("Parsing exception. Command:\n'" + output + "'\n", ex);
            }
        }

        /// <summary>
        /// Starts following the player's games.
        /// </summary>
        /// <param name="player">The player.</param>
        /// <returns>Game info and game state</returns>
        public async Task<ObserveGameResult> StartFollowingPlayer(Player player)
        {
            return await StartFollowingPlayer(player.Username);
        }

        /// <summary>
        /// Starts following the player's games.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <returns>Game info and game state</returns>
        public async Task<ObserveGameResult> StartFollowingPlayer(string username)
        {
            string output = await Execute(FicsCommand.FollowPlayer, username);

            try
            {
                StringReader reader = new StringReader(output);
                string line = reader.ReadLine();

                if (line.StartsWith("You will now be following " + username) && line.EndsWith("'s games."))
                {
                    // Check if player is currently playing a game
                    if (output.Count(c => c == '\n') > 2)
                    {
                        return ParseObserveGame(reader, ServerInterfaceVariables.DetailedGameInfo, ServerVariables.ShowProvisionalRatings);
                    }

                    return null;
                }
                else if (line == "You will not follow any player's games.")
                {
                    return null;
                }
                else
                {
                    throw new Exception(output);
                }
            }
            catch (Exception ex)
            {
                throw new AggregateException("Parsing exception. Command:\n'" + output + "'\n", ex);
            }
        }

        /// <summary>
        /// Stop following the last followed player.
        /// </summary>
        /// <param name="player">The player.</param>
        public async Task StopFollowingPlayer()
        {
            await StartFollowingPlayer("");
        }

        /// <summary>
        /// Stops observing the players game.
        /// </summary>
        /// <param name="player">The player.</param>
        public async Task StopObservingGame(Player player)
        {
            await StopObservingGame(player.Username);
        }

        /// <summary>
        /// Stops observing the game.
        /// </summary>
        /// <param name="game">The game.</param>
        public async Task StopObservingGame(Game game)
        {
            await StopObservingGame(game.Id.ToString());
        }

        /// <summary>
        /// Stops observing the game.
        /// </summary>
        /// <param name="gameId">The game id.</param>
        public async Task StopObservingGame(int gameId)
        {
            await StopObservingGame(gameId.ToString());
        }

        /// <summary>
        /// Stops observing the game.
        /// </summary>
        /// <param name="query">The query (username/game id).</param>
        public async Task StopObservingGame(string query)
        {
            string output = await Execute(FicsCommand.UnobserveGame, query);

            if (!output.StartsWith("Removing game ") || !output.EndsWith(" from observation list.\n"))
            {
                throw new Exception(output);
            }
        }

        /// <summary>
        /// Sends the message.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="message">The message.</param>
        public async Task SendMessage(string username, string message)
        {
            string output = await Execute(FicsCommand.SendMessage, username, message);

            if (output != "(told " + username + ")\n")
            {
                throw new Exception(output);
            }
        }

        /// <summary>
        /// Sends the message to the specified channel.
        /// </summary>
        /// <param name="channel">The channel.</param>
        /// <param name="message">The message.</param>
        /// <returns>Number of players that received the message</returns>
        public async Task<int> SendMessage(int channel, string message)
        {
            string output = await Execute(FicsCommand.SendMessage, channel, message);

            if (!output.StartsWith("(told "))
            {
                throw new Exception(output);
            }

            int userCountStart = output.IndexOf(' ');
            int userCountEnd = output.IndexOf(' ', userCountStart + 1);

            return int.Parse(output.Substring(userCountStart + 1, userCountEnd - userCountStart - 1));
        }

        /// <summary>
        /// Sends the shout message.
        /// </summary>
        /// <param name="message">The message.</param>
        public async Task SendShoutMessage(string message)
        {
            string output = await Execute(FicsCommand.SendShoutMessage, message);

            if (!output.StartsWith(Username + " shouts:"))
            {
                throw new Exception(output);
            }
        }

        /// <summary>
        /// Sends the chess shout message.
        /// </summary>
        /// <param name="message">The message.</param>
        public async Task SendChessShoutMessage(string message)
        {
            string output = await Execute(FicsCommand.SendChessShoutMessage, message);

            if (!output.StartsWith(Username + " cshouts:"))
            {
                throw new Exception(output);
            }
        }
        #endregion

        #region Sending and executing commands
        private List<FicsCommandState> executingCommands = new List<FicsCommandState>();

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
                    executingCommands.Add(new FicsCommandState());
                }

                executingCommands[commandNumber].IsExecuting = true;
                executingCommands[commandNumber].Command = command;
                return commandNumber + 1;
            }
        }

        internal async Task<FicsCommandState> Send(FicsCommand command, params object[] args)
        {
            string commandName = "$$" + command.GetSingleAttribute<ServerCommandNameAttribute>().Name;
            FicsCommandState commandState = null;

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
            FicsCommandState state = await Send(command, args);

            state.WaitForEnd();
            return state.Result;
        }
        #endregion

        protected override void LoginFinished()
        {
            base.LoginFinished();
            Send("iset block true").Wait();
            ivariables.SendCommandsAsBlock = true;
            var v1 = Task.Run(async () =>
            {
                MessageSplitter = "\n" + Prompt;
                await GetServerVariables(Username);
                MessageSplitter = "\n" + Prompt;
            });
            var v2 = GetServerInterfaceVariables(Username);
        }

        internal override bool IsKnownMessage(ref string message)
        {
            return TryParseCommandBlock(ref message)
                || TryParseGameStateChange(message)
                || TryParseGameEnded(message)
                || TryParseFollowedPlayerNewGame(message)
                || TryParseTextMessage(message);
        }

        #region TryParsing
        internal bool TryParseCommandBlock(ref string message)
        {
            if (message.Length > 0 && message[0] == CommandBlockStart)
            {
                // <BLOCK_START><command identifier><BLOCK_SEPARATOR><command code><BLOCK_SEPARATOR><command output><BLOCK_END>
                int messageEnd = message.IndexOf(CommandBlockEnd);
                Debug.Assert(messageEnd == message.Length - 1
                    || (messageEnd == message.Length - 2 && message[message.Length - 1] == '\n'));
                int firstBlockSeparator = message.IndexOf(CommandBlockSeparator);
                int secondBlockSeparator = message.IndexOf(CommandBlockSeparator, firstBlockSeparator + 1);
                int commandNumber = int.Parse(message.Substring(1, firstBlockSeparator - 1));
                int commandCode = int.Parse(message.Substring(firstBlockSeparator + 1, secondBlockSeparator - firstBlockSeparator - 1));
                string commandResult = message.Substring(secondBlockSeparator + 1, messageEnd - secondBlockSeparator - 1);

                FicsCommandState commandState = executingCommands[commandNumber - 1];

                if (commandState.Command == FicsCommand.NotSet)
                {
                    message = commandResult;
                    commandState.IsExecuting = false;
                    return false;
                }

                if (commandCode != commandState.CommandCode)
                {
                    throw new Exception("Unexpected command code " + commandCode);
                }

                commandState.Result = commandResult;
                commandState.IsExecuting = false;
                return true;
            }

            return false;
        }

        internal bool TryParseGameStateChange(string message)
        {
            // Check if it is game state change message (from observing a game)
            StringReader reader = new StringReader(message);

            if (message.StartsWith("\n<12>"))
            {
                if (GameStateChange != null)
                {
                    string emptyLine = reader.ReadLine();
                    string gameLine = reader.ReadLine();
                    string piecesLine = reader.ReadLine();

                    Debug.Assert(string.IsNullOrEmpty(emptyLine));

                    GameState gameState = ParseGameState(gameLine, piecesLine);

                    Task.Run(() => { GameStateChange(gameState); });
                }

                return true;
            }
            else if (message.StartsWith("\n<b1>"))
            {
                if (GameStateChange != null)
                {
                    string emptyLine = reader.ReadLine();
                    string piecesLine = reader.ReadLine();

                    Debug.Assert(string.IsNullOrEmpty(emptyLine));

                    GameState gameState = ParseGameState(null, piecesLine);

                    Task.Run(() => { GameStateChange(gameState); });
                }

                return true;
            }

            return false;
        }

        internal bool TryParseGameEnded(string message)
        {
            if (message.StartsWith("\n{Game "))
            {
                GameEndedInfo info = new GameEndedInfo();
                int position = 7;

                // Game id
                int gameIdEnd = message.IndexOf(' ', position);

                info.GameId = int.Parse(message.Substring(position, gameIdEnd - position));
                position = gameIdEnd + 1;

                // Players
                Debug.Assert(message[position] == '(');

                int playersEnd = message.IndexOf(')');
                string[] players = message.Substring(position + 1, playersEnd - position - 1).Split(new string[] { " vs. " }, StringSplitOptions.None);

                info.WhitePlayerUsername = players[0];
                info.BlackPlayerUsername = players[1];
                position = playersEnd + 1;

                // Message
                Debug.Assert(message[position] == ' ');
                position++;

                int messageEnd = message.IndexOf('}', position);

                info.Message = message.Substring(position, messageEnd - position);
                position = messageEnd + 1;

                // Points
                Debug.Assert(message[position] == ' ');
                position++;
                string[] points = message.Substring(position).Trim().Split("-".ToCharArray());
                if (points.Length == 1 && points[0] == "*")
                {
                    info.WhitePlayerPoints = 0;
                    info.BlackPlayerPoints = 0;
                }
                else
                {
                    info.WhitePlayerPoints = double.Parse(points[0]);
                    info.BlackPlayerPoints = double.Parse(points[1]);
                }

                if (GameEnded != null)
                {
                    Task.Run(() => { GameEnded(info); });
                }

                return true;
            }

            if (message.StartsWith("\nRemoving game ") && message.EndsWith(" from observation list.\n"))
            {
                const int start = 15, end = 23;
                int gameId = int.Parse(message.Substring(start, message.Length - end - start));

                if (GameStoppedObserving != null)
                {
                    Task.Run(() => { GameStoppedObserving(gameId); });
                }

                return true;
            }

            return false;
        }

        internal bool TryParseFollowedPlayerNewGame(string message)
        {
            StringReader reader = new StringReader(message);
            string line = reader.ReadLine();

            if (string.IsNullOrEmpty(line))
            {
                line = reader.ReadLine();
                if (line.Contains(", whom you are following, has started a game with "))
                {
                    ObserveGameResult game = ParseObserveGame(reader, ServerInterfaceVariables.DetailedGameInfo, ServerVariables.ShowProvisionalRatings);

                    if (FollowedPlayerStartedGame != null)
                    {
                        Task.Run(() => { FollowedPlayerStartedGame(game); });
                    }

                    return true;
                }
            }

            return false;
        }

        internal bool TryParseTextMessage(string message)
        {
            // Check if it is a text message
            StringReader reader = new StringReader(message);
            int position = 0;
            string line = reader.ReadLine();

            if (string.IsNullOrEmpty(line))
            {
                line = reader.ReadLine();
                AccountStatus accountStatus;
                string username = ParsePlayerAccountWithStatus(line, ref position, out accountStatus);

                if (username != null)
                {
                    string restOfLine = message.Substring(position + 1);

                    // Direct message
                    const string tellsYou = " tells you: ";
                    if (restOfLine.StartsWith(tellsYou))
                    {
                        string messageText = restOfLine.Substring(tellsYou.Length);

                        if (MessageReceived != null)
                        {
                            Task.Run(() => { MessageReceived(username, FixMessage(messageText)); });
                        }

                        return true;
                    }

                    // Channel message
                    if (restOfLine.Length > 0 && restOfLine[0] == '(')
                    {
                        int endOfChannelNumber = position = 1;

                        while (endOfChannelNumber < restOfLine.Length && char.IsDigit(restOfLine[endOfChannelNumber]))
                        {
                            endOfChannelNumber++;
                        }

                        if (position < endOfChannelNumber)
                        {
                            string s = restOfLine.Substring(endOfChannelNumber);

                            if (s.StartsWith("): "))
                            {
                                if (ChannelMessageReceived != null)
                                {
                                    int channel = int.Parse(restOfLine.Substring(position, endOfChannelNumber - position));

                                    Task.Run(() => { ChannelMessageReceived(channel, username, FixMessage(s.Substring(3))); });
                                }

                                return true;
                            }
                        }
                    }

                    // Shout messages
                    const string shouts = " shouts: ";

                    if (restOfLine.StartsWith(shouts))
                    {
                        string messageText = restOfLine.Substring(shouts.Length);

                        if (ShoutMessageReceived != null)
                        {
                            Task.Run(() => { ShoutMessageReceived(username, FixMessage(messageText)); });
                        }

                        return true;
                    }

                    // Chess Shout messages
                    const string cshouts = " cshouts: ";

                    if (restOfLine.StartsWith(cshouts))
                    {
                        string messageText = restOfLine.Substring(cshouts.Length);

                        if (ChessShoutMessageReceived != null)
                        {
                            Task.Run(() => { ChessShoutMessageReceived(username, FixMessage(messageText)); });
                        }

                        return true;
                    }

                    Regex ratingGameRegex = new Regex(@"^\((\d+|----|\+\+\+\+)\)\[(\d+)\]", RegexOptions.Singleline);
                    Match match = ratingGameRegex.Match(restOfLine);

                    if (match.Success)
                    {
                        Player player = new Player();
                        int gameId = int.Parse(match.Groups[2].Value);

                        position = match.Length;
                        player.Username = username;
                        player.AccountStatus = accountStatus;
                        player.Rating = ParseRating(match.Groups[1].Value, false);

                        // Whisper message
                        const string whispers = " whispers: ";

                        if (restOfLine.Length >= position + whispers.Length && restOfLine.Substring(position, whispers.Length) == whispers)
                        {
                            string messageText = restOfLine.Substring(position + whispers.Length);

                            if (Whisper != null)
                            {
                                Task.Run(() => { Whisper(player, gameId, FixMessage(messageText)); });
                            }

                            return true;
                        }

                        // Kibitz message
                        const string kibitzes = " kibitzes: ";

                        if (restOfLine.Length >= position + kibitzes.Length && restOfLine.Substring(position, kibitzes.Length) == kibitzes)
                        {
                            string messageText = restOfLine.Substring(position + kibitzes.Length);

                            if (Kibitz != null)
                            {
                                Task.Run(() => { Kibitz(player, gameId, FixMessage(messageText)); });
                            }

                            return true;
                        }
                    }

                    // Seeking message
                    Regex seekingRegex = new Regex(@"^ \((\d+|----|\+\+\+\+)\) seeking (\d+) (\d+) (rated|unrated) (blitz|lightning|untimed|standard|wild|atomic|crazyhouse|bughouse|losers|suicide|nonstandard) \(""play (\d+)"" to respond\)", RegexOptions.Singleline);

                    match = seekingRegex.Match(restOfLine);
                    if (match.Success)
                    {
                        Player player = new Player();
                        SeekInfo seekInfo = new SeekInfo();
                        int clockStartMinutes = int.Parse(match.Groups[2].Value);
                        int timeIncrementSeconds = int.Parse(match.Groups[3].Value);
                        string rated = match.Groups[4].Value;
                        string gameType = match.Groups[5].Value;

                        position = match.Length;
                        player.Username = username;
                        player.AccountStatus = accountStatus;
                        player.Rating = ParseRating(match.Groups[1].Value, false);
                        seekInfo.Id = int.Parse(match.Groups[6].Value);
                        seekInfo.Player = player;
                        seekInfo.ClockStart = TimeSpan.FromMinutes(clockStartMinutes);
                        seekInfo.TimeIncrement = TimeSpan.FromSeconds(timeIncrementSeconds);
                        seekInfo.GameType = ParseEnum<GameType>(gameType);
                        seekInfo.Rated = rated == "rated";

                        if (Seeking != null)
                        {
                            Task.Run(() => { Seeking(seekInfo); });
                        }

                        return true;
                    }
                }
                else // username == null
                {
                    // Announcement message
                    const string AnnouncementStart = "**ANNOUNCEMENT**";
                    string trimmedMessage = message.Trim();

                    if (trimmedMessage.StartsWith(AnnouncementStart))
                    {
                        if (Announcement != null)
                        {
                            Task.Run(() => { Announcement(FixMessage(trimmedMessage)); });
                        }

                        return true;
                    }
                }
            }

            return false;
        }

        private static string FixMessage(string message)
        {
            return message.Replace("\n\\   ", "");
        }

        #endregion

        #region Parsing
        internal static IList<Game> ParseGames(string output)
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
            Debug.Assert(line.StartsWith("  " + (games.Count - games.Count(g => g.Type == GameType.Bughouse && !g.InSetup && !g.Examined) / 2) + " game"));
            line = reader.ReadLine();
            Debug.Assert(string.IsNullOrEmpty(line));

            return games;
        }

        internal static ObserveGameResult ParseObserveGame(StringReader reader, bool hasDetailedGameInfo, bool hasProvisionalRatings)
        {
            ObserveGameResult result = new ObserveGameResult();
            string line = reader.ReadLine();

            if (!line.StartsWith("You are now observing game "))
            {
                throw new Exception("Parsing wrong output for ObserveGame");
            }

            int gameIdEnd = line.IndexOf('.');
            int gameId = int.Parse(line.Substring(27, gameIdEnd - 27));

            // Parse game info
            string shortGameInfo = reader.ReadLine();
            string detailedGameInfo = null;

            Debug.Assert(shortGameInfo.StartsWith("Game " + gameId + ":"));
            line = reader.ReadLine();
            Debug.Assert(string.IsNullOrEmpty(line));

            if (hasDetailedGameInfo)
            {
                detailedGameInfo = reader.ReadLine();
                line = reader.ReadLine();
                Debug.Assert(string.IsNullOrEmpty(line));
            }

            // Take game info
            result.GameInfo = ParseGameInfo(shortGameInfo, detailedGameInfo, hasProvisionalRatings);

            // Parse game state (style 12)
            string style12gameLine = reader.ReadLine();
            string style12piecesLine = reader.ReadLine();

            result.GameState = ParseGameState(style12gameLine, style12piecesLine);
            return result;
        }
        internal static DateTime ParseDateTime(string dateTimeString)
        {
            return DateTimeOffset.ParseExact(dateTimeString.Replace(" EDT", " -04:00"), "ddd MMM dd, HH:mm zzzz yyyy", CultureInfo.InvariantCulture).UtcDateTime;
        }

        internal static ChessMoveList ParseMoveList(string output)
        {
            ChessMoveList moveList = new ChessMoveList();
            StringReader reader = new StringReader(output);
            string line = reader.ReadLine();

            // Game id
            Debug.Assert(string.IsNullOrEmpty(line));
            line = reader.ReadLine();
            Debug.Assert(line.StartsWith("Movelist for game ") && line.EndsWith(":"));
            moveList.GameId = int.Parse(line.Substring(18, line.Length - 19));
            line = reader.ReadLine();
            Debug.Assert(string.IsNullOrEmpty(line));
            line = reader.ReadLine();

            // DateTime
            string[] tokens = line.Split(new string[] { " --- " }, StringSplitOptions.None);
            Debug.Assert(tokens.Length == 2);
            moveList.GameStarted = ParseDateTime(tokens[1]);

            // White player username
            tokens = tokens[0].Split(new string[] { " vs. " }, StringSplitOptions.None);
            Debug.Assert(tokens.Length == 2);
            int position = 0;
            AccountStatus accountStatus;
            moveList.WhitePlayer = new Player();
            moveList.WhitePlayer.Username = ParsePlayerAccountWithStatus(tokens[0], ref position, out accountStatus);
            moveList.WhitePlayer.AccountStatus = accountStatus;

            // White player rating
            SkipWhiteSpaces(tokens[0], ref position);
            Debug.Assert(tokens[0][position] == '(');
            position++;
            moveList.WhitePlayer.Rating = ParseRating(tokens[0], ref position, false);

            // Black player username
            position = 0;
            moveList.BlackPlayer = new Player();
            moveList.BlackPlayer.Username = ParsePlayerAccountWithStatus(tokens[1], ref position, out accountStatus);
            moveList.BlackPlayer.AccountStatus = accountStatus;

            // White player rating
            SkipWhiteSpaces(tokens[1], ref position);
            Debug.Assert(tokens[1][position] == '(');
            position++;
            moveList.BlackPlayer.Rating = ParseRating(tokens[1], ref position, false);

            // TODO: Parse game info
            line = reader.ReadLine();

            // Parse moves header
            line = reader.ReadLine();
            Debug.Assert(string.IsNullOrEmpty(line));
            line = reader.ReadLine();
            tokens = line.Split(" ".ToArray(), StringSplitOptions.RemoveEmptyEntries);
            Debug.Assert(tokens.Length == 3);
            Debug.Assert(tokens[0] == "Move");
            Debug.Assert(tokens[1] == moveList.WhitePlayer.Username);
            Debug.Assert(tokens[2] == moveList.BlackPlayer.Username);
            line = reader.ReadLine();
            tokens = line.Split(" ".ToArray(), StringSplitOptions.RemoveEmptyEntries);
            Debug.Assert(tokens.Length == 3);
            Debug.Assert(tokens[0].Count(c => c == '-') > 3);
            Debug.Assert(tokens[1].Count(c => c == '-') > 3);
            Debug.Assert(tokens[2].Count(c => c == '-') > 3);

            // Parse moves
            int currentMove = 1;
            line = reader.ReadLine();
            moveList.WhiteMoves = new List<ChessMove>();
            moveList.BlackMoves = new List<ChessMove>();
            while (!string.IsNullOrEmpty(line) && !line.Trim().StartsWith("{"))
            {
                tokens = line.Split(" ".ToArray(), StringSplitOptions.RemoveEmptyEntries);
                Debug.Assert(tokens.Length == 5 || tokens.Length == 3);

                // Parse move number
                Debug.Assert(tokens[0].EndsWith("."));
                Debug.Assert(int.Parse(tokens[0].Substring(0, tokens[0].Length - 1)) == currentMove);

                // Parse white move
                ChessMove whiteMove = new ChessMove();

                if (tokens[1] == "...")
                {
                    break;
                }

                whiteMove.Move = tokens[1];
                Debug.Assert(tokens[2].StartsWith("(") && tokens[2].EndsWith(")"));
                whiteMove.Time = ParseTime(tokens[2].Substring(1, tokens[2].Length - 2));
                moveList.WhiteMoves.Add(whiteMove);

                // Parse black move
                if (tokens.Length == 5)
                {
                    ChessMove blackMove = new ChessMove();

                    blackMove.Move = tokens[3];
                    Debug.Assert(tokens[4].StartsWith("(") && tokens[4].EndsWith(")"));
                    blackMove.Time = ParseTime(tokens[4].Substring(1, tokens[4].Length - 2));
                    moveList.BlackMoves.Add(blackMove);
                }

                line = reader.ReadLine();
                currentMove++;
            }

            return moveList;
        }

        internal static Game ParseGame(string line)
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
                game.BlackClock = ParseTime(line.Substring(position, nextSpaceIndex - position).Trim());
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

        internal static TEnum ParseEnum<TEnum>(string value) where TEnum : struct
        {
            foreach (TEnum enumValue in Enum.GetValues(typeof(TEnum)))
            {
                if (((Enum)(object)enumValue).GetSingleAttribute<ServerVariableNameAttribute>().Name == value
                    || enumValue.ToString().Equals(value, StringComparison.CurrentCultureIgnoreCase))
                {
                    return enumValue;
                }
            }

            throw new Exception("Unable to parse enum " + typeof(TEnum));
        }

        internal static string GenerateEnumString<TEnum>(TEnum options) where TEnum : struct
        {
            string result = "";

            foreach (TEnum option in Enum.GetValues(typeof(TEnum)))
            {
                if (((int)(object)options & (int)(object)option) == (int)(object)option)
                {
                    result += ((Enum)(object)option).GetSingleAttribute<ServerVariableNameAttribute>().Name;
                }
            }

            return result;
        }

        internal static TimeSpan ParseTime(string str)
        {
            return TimeSpan.ParseExact(str, new string[] { @"%h\:mm\:ss", @"%m\:ss", @"%h\:mm\:ss\.fff", @"%m\:ss\.fff" }, null);
        }

        internal static void SkipWhiteSpaces(string line, ref int position)
        {
            while (position < line.Length && char.IsWhiteSpace(line[position]))
            {
                position++;
            }
        }

        internal static Partnership ParsePartnership(string line, bool hasProvisionalRating)
        {
            Partnership partnership = new Partnership();
            int position = 0;

            partnership.Player1 = ParsePlayer(line, ref position, hasProvisionalRating);
            SkipWhiteSpaces(line, ref position);
            Debug.Assert(line[position] == '/');
            position++;
            partnership.Player2 = ParsePlayer(line, ref position, hasProvisionalRating);
            return partnership;
        }

        internal static IList<Player> ParsePlayers(TextReader reader, bool hasProvisionalRating)
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

        internal static Player ParsePlayer(string line, ref int position, bool hasProvisionalRating)
        {
            Player player = new Player();

            // Parse rating
            PlayerProvisionalRating provisionalRating = PlayerProvisionalRating.Unknown;

            player.Rating = ParseRating(line, ref position, hasProvisionalRating, ref provisionalRating);
            player.ProvisionalRating = provisionalRating;

            // Status
            player.Status = ParseEnum<PlayerStatus>(line.Substring(position++, 1));

            // Username and account status
            AccountStatus accountStatus;

            SkipWhiteSpaces(line, ref position);
            player.Username = ParsePlayerAccountWithStatus(line, ref position, out accountStatus);
            player.AccountStatus = accountStatus;
            return player;
        }

        internal static string ParsePlayerAccountWithStatus(string line, ref int position, out AccountStatus accountStatus)
        {
            accountStatus = AccountStatus.RegularAccount;

            int usernameEnd = position;
            while (usernameEnd < line.Length && (char.IsLetter(line[usernameEnd]) || line[usernameEnd] == '.' || line[usernameEnd] == '_' || line[usernameEnd] == ',' || char.IsDigit(line[usernameEnd])))
            {
                usernameEnd++;
            }

            if (usernameEnd == position)
            {
                return null;
            }

            string username = line.Substring(position, usernameEnd - position);
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
                            accountStatus |= status;
                            position += statusString.Length;
                            found = true;
                            break;
                        }
                    }
                }
            }

            return username;
        }

        internal static Dictionary<string, string> ParseVariablesCommand(string commandHeader, string command, out string username)
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

        internal static Dictionary<string, string> ParseVariables(TextReader reader)
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

        internal static GameInfo ParseGameInfo(string line, string detailedGameInfo, bool hasProvisionalRating)
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

        internal static int ParseRating(string line, bool hasProvisionalRating, ref PlayerProvisionalRating provisionalRating)
        {
            int position = 0;

            return ParseRating(line, ref position, hasProvisionalRating, ref provisionalRating);
        }

        internal static int ParseRating(string line, bool hasProvisionalRating)
        {
            int position = 0;

            return ParseRating(line, ref position, hasProvisionalRating);
        }

        internal static int ParseRating(string line, ref int position, bool hasProvisionalRating)
        {
            PlayerProvisionalRating provisionalRating = PlayerProvisionalRating.Unknown;

            return ParseRating(line, ref position, hasProvisionalRating, ref provisionalRating);
        }

        internal static int ParseRating(string line, ref int position, bool hasProvisionalRating, ref PlayerProvisionalRating provisionalRating)
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
            else if (line[position] == 'U')
            {
                Debug.Assert(line.Substring(position, 3) == "UNR");
                position += 3;
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

        internal static GameState ParseGameState(string gameLine, string piecesLine)
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
                                Type = ParseEnum<ChessPieceType>(char.ToUpper(piece).ToString()),
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

                state.WhitePieces = new List<ChessPieceType>();
                foreach (char piece in whitePieces)
                {
                    state.WhitePieces.Add(ParseEnum<ChessPieceType>(char.ToUpper(piece).ToString()));
                }

                state.BlackPieces = new List<ChessPieceType>();
                foreach (char piece in blackPieces)
                {
                    state.BlackPieces.Add(ParseEnum<ChessPieceType>(char.ToUpper(piece).ToString()));
                }
            }

            return state;
        }
        #endregion
    }
}
