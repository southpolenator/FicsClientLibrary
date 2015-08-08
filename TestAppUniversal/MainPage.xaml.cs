using Internet.Chess.Server.Fics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

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
            ChessBoard[0, 0] = new ChessPieceWithColor() { Color = ChessPieceColor.White, Type = ChessPieceType.Bishop };
            ChessBoard[0, 1] = new ChessPieceWithColor() { Color = ChessPieceColor.White, Type = ChessPieceType.King };
            ChessBoard[0, 2] = new ChessPieceWithColor() { Color = ChessPieceColor.White, Type = ChessPieceType.Knight };
            ChessBoard[0, 3] = new ChessPieceWithColor() { Color = ChessPieceColor.White, Type = ChessPieceType.Pawn };
            ChessBoard[0, 4] = new ChessPieceWithColor() { Color = ChessPieceColor.White, Type = ChessPieceType.Queen };
            ChessBoard[0, 5] = new ChessPieceWithColor() { Color = ChessPieceColor.White, Type = ChessPieceType.Rook };
            ChessBoard[1, 0] = new ChessPieceWithColor() { Color = ChessPieceColor.Black, Type = ChessPieceType.Bishop };
            ChessBoard[1, 1] = new ChessPieceWithColor() { Color = ChessPieceColor.Black, Type = ChessPieceType.King };
            ChessBoard[1, 2] = new ChessPieceWithColor() { Color = ChessPieceColor.Black, Type = ChessPieceType.Knight };
            ChessBoard[1, 3] = new ChessPieceWithColor() { Color = ChessPieceColor.Black, Type = ChessPieceType.Pawn };
            ChessBoard[1, 4] = new ChessPieceWithColor() { Color = ChessPieceColor.Black, Type = ChessPieceType.Queen };
            ChessBoard[1, 5] = new ChessPieceWithColor() { Color = ChessPieceColor.Black, Type = ChessPieceType.Rook };
            Task.Run(() => { StartFics(); });
        }

        private async void StartFics()
        {
            try
            {
                fics = new FicsClient();
                await fics.LoginGuest();

                var games = await fics.ListGames();

                await GamesList.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    Dictionary<GameType, ListView> typeGames = new Dictionary<GameType, ListView>();

                    foreach (var game in games.OrderByDescending(g => g.WhitePlayer.Rating + g.BlackPlayer.Rating))
                    {
                        ListView listView;

                        if (!typeGames.TryGetValue(game.Type, out listView))
                        {
                            listView = new ListView();
                            stackPanel.Children.Add(listView);
                            listView.Visibility = Visibility.Collapsed;
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

                    GamesList.SelectionChanged += (o, e) =>
                    {
                        foreach (var typeGame in typeGames)
                        {
                            typeGame.Value.Visibility = Visibility.Collapsed;
                        }

                        dynamic text = GamesList.SelectedItem;

                        typeGames[(GameType)text.GameType].Visibility = Visibility.Visible;
                    };
                    GamesList.Visibility = Visibility.Visible;
                    LoadingProgress.Visibility = Visibility.Collapsed;
                });
            }
            catch (Exception ex)
            {
            }
        }
    }
}
