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

        public MainPage()
        {
            InitializeComponent();
            NavigationCacheMode = NavigationCacheMode.Required; // Don't reload, but cache page when navigating
            App.Current.FicsClientReady += OnFicsClientReady;


            GameControl.BlackShouldBeDown = false;

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

                await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    Dictionary<GameType, ListView> typeGames = new Dictionary<GameType, ListView>();

                    ChildGamesPanel.Children.Clear();
                    GamesList.Items.Clear();
                    foreach (var game in games.OrderByDescending(g => g.WhitePlayer.Rating + g.BlackPlayer.Rating))
                    {
                        ListView listView;

                        if (!typeGames.TryGetValue(game.Type, out listView))
                        {
                            listView = new ListView();
                            listView.Visibility = Visibility.Collapsed;
                            listView.IsItemClickEnabled = true;
                            listView.ItemClick += (o, e) =>
                            {
                                dynamic gameText = e.ClickedItem;
                                Game g = gameText.Game;

                                Frame rootFrame = Window.Current.Content as Frame;
                                rootFrame.Navigate(typeof(GamePage), g);
                            };
                            ChildGamesPanel.Children.Add(listView);
                            typeGames.Add(game.Type, listView);
                        }

                        dynamic text = new ToStringExpandoObject();

                        text.ToString = new ToStringFunc(() =>
                        {
                            return string.Format("{4}: {0} ({1})  VS  {2} ({3})", game.WhitePlayer.Username, game.WhitePlayer.RatingString, game.BlackPlayer.Username, game.BlackPlayer.RatingString, game.Rated ? "Rated" : "Unrated");
                        });
                        text.Game = game;
                        listView.Items.Add(text);
                    }

                    foreach (var typeGame in typeGames.OrderBy(kvp => kvp.Key.ToString()))
                    {
                        dynamic text = new ToStringExpandoObject();

                        text.ToString = new ToStringFunc(() =>
                        {
                            return string.Format("{0} ({1})", typeGame.Key, typeGame.Value.Items.Count);
                        });
                        text.GameType = typeGame.Key;

                        GamesList.Items.Add(text);
                    }

                    GamesList.IsItemClickEnabled = true;
                    GamesList.ItemClick += (o, e) =>
                    {
                        foreach (var typeGame in typeGames)
                        {
                            typeGame.Value.Visibility = Visibility.Collapsed;
                        }

                        dynamic text = e.ClickedItem;

                        typeGames[(GameType)text.GameType].Visibility = Visibility.Visible;
                    };

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
