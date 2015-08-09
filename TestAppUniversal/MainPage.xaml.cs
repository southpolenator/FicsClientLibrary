using Internet.Chess.Server.Fics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace TestAppUniversal
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private FicsClient fics;
        private Dictionary<GameType, ListView> gameTypeGameLists = new Dictionary<GameType, ListView>();

        public MainPage()
        {
            InitializeComponent();
            NavigationCacheMode = NavigationCacheMode.Required; // Don't reload, but cache page when navigating
            App.Current.FicsClientReady += OnFicsClientReady;

            foreach (GameType gameType in Enum.GetValues(typeof(GameType)).OfType<GameType>().OrderBy(o => o.ToString()))
            {
                ListView listView = new ListView();
                listView.Visibility = Visibility.Collapsed;
                listView.IsItemClickEnabled = true;
                listView.ItemClick += (o, e) =>
                {
                    dynamic gameText = e.ClickedItem;
                    Game g = gameText.Game;

                    Frame rootFrame = Window.Current.Content as Frame;
                    rootFrame.Navigate(typeof(GamePage), g);
                };
                GamesList.Items.Clear();
                ChildGamesPanel.Children.Add(listView);
                gameTypeGameLists.Add(gameType, listView);
            }

            foreach (var typeGame in gameTypeGameLists.OrderBy(kvp => kvp.Key.ToString()))
            {
                UpdateGamesListItem(typeGame);
            }

            GamesList.IsItemClickEnabled = true;
            GamesList.ItemClick += (o, e) =>
            {
                foreach (var typeGame in gameTypeGameLists)
                {
                    typeGame.Value.Visibility = Visibility.Collapsed;
                }

                dynamic text = e.ClickedItem;

                gameTypeGameLists[(GameType)text.GameType].Visibility = Visibility.Visible;
            };


#if DEBUG
            GameControl GameControl = new GameControl();

            GameControl.VerticalAlignment = VerticalAlignment.Bottom;
            GameControl.HorizontalAlignment = HorizontalAlignment.Center;
            GameControl.BlackShouldBeDown = false;
            GameControl.Width = 600;
            GameControl.Height = 600;
            Grid.Children.Insert(0, GameControl);

            GameInfo gameInfo = new GameInfo();

            gameInfo.WhitePlayer = new Player()
            {
                Username = "WhitePlayer",
                Rating = 1500,
            };
            gameInfo.BlackPlayer = new Player()
            {
                Username = "BlackPlayer",
                Rating = 1500,
            };
            gameInfo.Type = GameType.Crazyhouse;
            GameControl.InitializeGameInfo(gameInfo);

            GameState gameState = new GameState();

            gameState.Board = new ChessPieceWithColor[8, 8];
            gameState.Board[0, 0] = ChessPieceWithColor.BlackRook;
            gameState.Board[0, 1] = ChessPieceWithColor.BlackKnight;
            gameState.Board[0, 2] = ChessPieceWithColor.BlackBishop;
            gameState.Board[0, 3] = ChessPieceWithColor.BlackQueen;
            gameState.Board[0, 4] = ChessPieceWithColor.BlackKing;
            gameState.Board[0, 5] = ChessPieceWithColor.BlackBishop;
            gameState.Board[0, 6] = ChessPieceWithColor.BlackKnight;
            gameState.Board[0, 7] = ChessPieceWithColor.BlackRook;
            gameState.Board[1, 0] = ChessPieceWithColor.BlackPawn;
            gameState.Board[1, 1] = ChessPieceWithColor.BlackPawn;
            gameState.Board[1, 2] = ChessPieceWithColor.BlackPawn;
            gameState.Board[1, 3] = ChessPieceWithColor.BlackPawn;
            gameState.Board[1, 4] = ChessPieceWithColor.BlackPawn;
            gameState.Board[1, 5] = ChessPieceWithColor.BlackPawn;
            gameState.Board[1, 6] = ChessPieceWithColor.BlackPawn;
            gameState.Board[1, 7] = ChessPieceWithColor.BlackPawn;
            gameState.Board[6, 0] = ChessPieceWithColor.WhitePawn;
            gameState.Board[6, 1] = ChessPieceWithColor.WhitePawn;
            gameState.Board[6, 2] = ChessPieceWithColor.WhitePawn;
            gameState.Board[6, 3] = ChessPieceWithColor.WhitePawn;
            gameState.Board[6, 4] = ChessPieceWithColor.WhitePawn;
            gameState.Board[6, 5] = ChessPieceWithColor.WhitePawn;
            gameState.Board[6, 6] = ChessPieceWithColor.WhitePawn;
            gameState.Board[6, 7] = ChessPieceWithColor.WhitePawn;
            gameState.Board[7, 0] = ChessPieceWithColor.WhiteRook;
            gameState.Board[7, 1] = ChessPieceWithColor.WhiteKnight;
            gameState.Board[7, 2] = ChessPieceWithColor.WhiteBishop;
            gameState.Board[7, 3] = ChessPieceWithColor.WhiteQueen;
            gameState.Board[7, 4] = ChessPieceWithColor.WhiteKing;
            gameState.Board[7, 5] = ChessPieceWithColor.WhiteBishop;
            gameState.Board[7, 6] = ChessPieceWithColor.WhiteKnight;
            gameState.Board[7, 7] = ChessPieceWithColor.WhiteRook;
            gameState.WhiteClock = TimeSpan.FromMilliseconds(10436);
            gameState.BlackClock = TimeSpan.FromMilliseconds(9456);
            gameState.WhiteMove = true;
            gameState.WhitePieces = new List<ChessPieceType>() { ChessPieceType.Pawn, ChessPieceType.Pawn, ChessPieceType.Rook, ChessPieceType.Rook };
            gameState.BlackPieces = new List<ChessPieceType>() { ChessPieceType.Pawn, ChessPieceType.Knight };
            gameState.LastMoveVerbose = "Q/d1-a4";

            GameControl.OnGameStateChanged(gameState);
#endif
        }

        private void UpdateGamesListItem(KeyValuePair<GameType, ListView> typeGame)
        {
            dynamic text = new ToStringExpandoObject();

            text.ToString = new ToStringFunc(() =>
            {
                return string.Format("{0} ({1})", typeGame.Key, typeGame.Value.Items.Count);
            });
            text.GameType = typeGame.Key;

            for (int i = 0; i < GamesList.Items.Count; i++)
            {
                dynamic t = GamesList.Items[i];

                if (t.GameType == text.GameType)
                {
                    GamesList.Items[i] = text;
                    return;
                }
            }

            GamesList.Items.Add(text);
        }

        private void OnFicsClientReady(FicsClient client)
        {
            App.Current.FicsClientReady -= OnFicsClientReady;
            fics = client;
            Task.Run(() => { RefreshGames(); });
        }

        private async void RefreshGames()
        {
            try
            {
                await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    ProgressText.Text = "Loading games";
                });

                var games = await fics.ListGames();

                foreach (GameType gameType in Enum.GetValues(typeof(GameType)))
                {
                    List<dynamic> gameTexts = new List<dynamic>();
                    ListView listView = gameTypeGameLists[gameType];

                    await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                    {
                        listView.Items.Clear();
                    });

                    foreach (var game in games.Where(g => g.Type == gameType).OrderByDescending(g => g.WhitePlayer.Rating + g.BlackPlayer.Rating))
                    {
                        dynamic text = new ToStringExpandoObject();

                        text.ToString = new ToStringFunc(() =>
                        {
                            return string.Format("{4}: {0} ({1})  VS  {2} ({3})", game.WhitePlayer.Username, game.WhitePlayer.RatingString, game.BlackPlayer.Username, game.BlackPlayer.RatingString, game.Rated ? "Rated" : "Unrated");
                        });
                        text.Game = game;
                        gameTexts.Add(text);

                        if (gameTexts.Count >= 10)
                        {
                            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                            {
                                foreach (dynamic t in gameTexts)
                                    listView.Items.Add(t);
                            });

                            gameTexts.Clear();
                        }
                    }

                    await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                    {
                        foreach (dynamic text in gameTexts)
                            listView.Items.Add(text);
                    });
                }

                await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    foreach (var typeGame in gameTypeGameLists)
                    {
                        UpdateGamesListItem(typeGame);
                    }

                    GamesPanel.Visibility = Visibility.Visible;
                    ProgressPanel.Visibility = Visibility.Collapsed;
                });
            }
            catch (Exception ex)
            {
            }
        }
    }
}
