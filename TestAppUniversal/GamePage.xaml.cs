using Internet.Chess.Server.Fics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace TestAppUniversal
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class GamePage : Page
    {
        private FicsClient fics;
        private Game game;

        public GamePage()
        {
            this.InitializeComponent();
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

        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            game = (Game)e.Parameter;
            App.Current.FicsClientReady += OnFicsClientReady;
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            if (fics != null)
            {
                var t = fics.StopObservingGame(game);
                fics.GameStateChange -= OnGameStateChanged;
            }
        }

        private void OnFicsClientReady(FicsClient client)
        {
            App.Current.FicsClientReady -= OnFicsClientReady;
            fics = client;
            fics.GameStateChange += OnGameStateChanged;
            Task.Run(() => { StartGame(); });
        }

        private async void StartGame()
        {
            try
            {
                var result = await fics.StartObservingGame(game);

                if (result.GameInfo.BlackPlayer.Username != game.BlackPlayer.Username
                    || result.GameInfo.WhitePlayer.Username != game.WhitePlayer.Username)
                {
                    // TODO: Stop observing wrong game
                }

                OnGameStateChanged(result.GameState);
            }
            catch (Exception ex)
            {
                // TODO: Notify user that game is not playing
            }
        }

        private void OnGameStateChanged(GameState gameState)
        {
            if (gameState.GameId == game.Id)
            {
                var t = Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    if (gameState.Board != null)
                        for (int y = 0; y < 8; y++)
                            for (int x = 0; x < 8; x++)
                                ChessBoard[y, x] = gameState.Board[y, x];
                });
            }
        }
    }
}
