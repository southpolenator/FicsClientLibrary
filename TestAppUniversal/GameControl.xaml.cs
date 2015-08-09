using Internet.Chess.Server.Fics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace TestAppUniversal
{
    public sealed partial class GameControl : UserControl
    {
        private Brush originalTextColor;
        private DispatcherTimer dispatcherTimer;
        private DateTime lastTick;

        public GameControl()
        {
            dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Tick += OnTimer;
            dispatcherTimer.Interval = TimeSpan.FromMilliseconds(100);
            this.InitializeComponent();
            originalTextColor = WhitePlayer.Foreground;
            DrawPieces(true);
            SetWhiteQueenCount(0);
            SetWhiteRookCount(0);
            SetWhiteKnightCount(0);
            SetWhiteBishopCount(0);
            SetWhitePawnCount(0);
            SetBlackQueenCount(0);
            SetBlackRookCount(0);
            SetBlackKnightCount(0);
            SetBlackBishopCount(0);
            SetBlackPawnCount(0);
        }

    public bool IsWhiteMove
        {
            get { return WhitePlayerSymbol.Opacity == 1; }
        }

        public bool BlackShouldBeDown
        {
            get { return ChessBoard.Flipped; }
            set { ChessBoard.Flipped = value; }
        }

        #region Control mappings to avoid flipping everywhere in code
        private TextBlock BlackPlayer { get { return BlackShouldBeDown ? whitePlayer : blackPlayer; } }
        private SymbolIcon BlackPlayerSymbol { get { return BlackShouldBeDown ? whitePlayerSymbol : blackPlayerSymbol; } }
        private TextBlock BlackPlayerTime { get { return BlackShouldBeDown ? whitePlayerTime : blackPlayerTime; } }
        private Canvas BlackPlayerQueen { get { return BlackShouldBeDown ? whitePlayerQueen : blackPlayerQueen; } }
        private TextBlock BlackPlayerQueenCount { get { return BlackShouldBeDown ? whitePlayerQueenCount : blackPlayerQueenCount; } }
        private Canvas BlackPlayerRook { get { return BlackShouldBeDown ? whitePlayerRook : blackPlayerRook; } }
        private TextBlock BlackPlayerRookCount { get { return BlackShouldBeDown ? whitePlayerRookCount : blackPlayerRookCount; } }
        private Canvas BlackPlayerKnight { get { return BlackShouldBeDown ? whitePlayerKnight : blackPlayerKnight; } }
        private TextBlock BlackPlayerKnightCount { get { return BlackShouldBeDown ? whitePlayerKnightCount : blackPlayerKnightCount; } }
        private Canvas BlackPlayerBishop { get { return BlackShouldBeDown ? whitePlayerBishop : blackPlayerBishop; } }
        private TextBlock BlackPlayerBishopCount { get { return BlackShouldBeDown ? whitePlayerBishopCount : blackPlayerBishopCount; } }
        private Canvas BlackPlayerPawn { get { return BlackShouldBeDown ? whitePlayerPawn : blackPlayerPawn; } }
        private TextBlock BlackPlayerPawnCount { get { return BlackShouldBeDown ? whitePlayerPawnCount : blackPlayerPawnCount; } }

        private TextBlock WhitePlayer { get { return !BlackShouldBeDown ? whitePlayer : blackPlayer; } }
        private SymbolIcon WhitePlayerSymbol { get { return !BlackShouldBeDown ? whitePlayerSymbol : blackPlayerSymbol; } }
        private TextBlock WhitePlayerTime { get { return !BlackShouldBeDown ? whitePlayerTime : blackPlayerTime; } }
        private Canvas WhitePlayerQueen { get { return !BlackShouldBeDown ? whitePlayerQueen : blackPlayerQueen; } }
        private TextBlock WhitePlayerQueenCount { get { return !BlackShouldBeDown ? whitePlayerQueenCount : blackPlayerQueenCount; } }
        private Canvas WhitePlayerRook { get { return !BlackShouldBeDown ? whitePlayerRook : blackPlayerRook; } }
        private TextBlock WhitePlayerRookCount { get { return !BlackShouldBeDown ? whitePlayerRookCount : blackPlayerRookCount; } }
        private Canvas WhitePlayerKnight { get { return !BlackShouldBeDown ? whitePlayerKnight : blackPlayerKnight; } }
        private TextBlock WhitePlayerKnightCount { get { return !BlackShouldBeDown ? whitePlayerKnightCount : blackPlayerKnightCount; } }
        private Canvas WhitePlayerBishop { get { return !BlackShouldBeDown ? whitePlayerBishop : blackPlayerBishop; } }
        private TextBlock WhitePlayerBishopCount { get { return !BlackShouldBeDown ? whitePlayerBishopCount : blackPlayerBishopCount; } }
        private Canvas WhitePlayerPawn { get { return !BlackShouldBeDown ? whitePlayerPawn : blackPlayerPawn; } }
        private TextBlock WhitePlayerPawnCount { get { return !BlackShouldBeDown ? whitePlayerPawnCount : blackPlayerPawnCount; } }
        #endregion

        public TimeSpan WhiteClock { get; private set; }

        public TimeSpan BlackClock { get; private set; }

        private void OnTimer(object sender, object e)
        {
            TimeSpan elapsed = DateTime.UtcNow - lastTick;

            lastTick = DateTime.UtcNow;
            if (IsWhiteMove)
            {
                WhiteClock -= elapsed;
                ShowTime(WhitePlayerTime, WhiteClock);
            }
            else
            {
                BlackClock -= elapsed;
                ShowTime(BlackPlayerTime, BlackClock);
            }
        }

        public void InitializeGameInfo(GameInfo info)
        {
            WhitePlayer.Text = info.WhitePlayer.ToString();
            BlackPlayer.Text = info.BlackPlayer.ToString();
            DrawPieces(info.Type != GameType.Crazyhouse && info.Type != GameType.Bughouse);
        }

        private void DrawPieces(bool inverted)
        {
            if (!inverted)
            {
                ChessPieceGraphics.DrawPiece(WhitePlayerQueen, ChessPieceWithColor.WhiteQueen);
                ChessPieceGraphics.DrawPiece(WhitePlayerRook, ChessPieceWithColor.WhiteRook);
                ChessPieceGraphics.DrawPiece(WhitePlayerKnight, ChessPieceWithColor.WhiteKnight);
                ChessPieceGraphics.DrawPiece(WhitePlayerBishop, ChessPieceWithColor.WhiteBishop);
                ChessPieceGraphics.DrawPiece(WhitePlayerPawn, ChessPieceWithColor.WhitePawn);
                ChessPieceGraphics.DrawPiece(BlackPlayerQueen, ChessPieceWithColor.BlackQueen);
                ChessPieceGraphics.DrawPiece(BlackPlayerRook, ChessPieceWithColor.BlackRook);
                ChessPieceGraphics.DrawPiece(BlackPlayerKnight, ChessPieceWithColor.BlackKnight);
                ChessPieceGraphics.DrawPiece(BlackPlayerBishop, ChessPieceWithColor.BlackBishop);
                ChessPieceGraphics.DrawPiece(BlackPlayerPawn, ChessPieceWithColor.BlackPawn);
            }
            else
            {
                ChessPieceGraphics.DrawPiece(WhitePlayerQueen, ChessPieceWithColor.BlackQueen);
                ChessPieceGraphics.DrawPiece(WhitePlayerRook, ChessPieceWithColor.BlackRook);
                ChessPieceGraphics.DrawPiece(WhitePlayerKnight, ChessPieceWithColor.BlackKnight);
                ChessPieceGraphics.DrawPiece(WhitePlayerBishop, ChessPieceWithColor.BlackBishop);
                ChessPieceGraphics.DrawPiece(WhitePlayerPawn, ChessPieceWithColor.BlackPawn);
                ChessPieceGraphics.DrawPiece(BlackPlayerQueen, ChessPieceWithColor.WhiteQueen);
                ChessPieceGraphics.DrawPiece(BlackPlayerRook, ChessPieceWithColor.WhiteRook);
                ChessPieceGraphics.DrawPiece(BlackPlayerKnight, ChessPieceWithColor.WhiteKnight);
                ChessPieceGraphics.DrawPiece(BlackPlayerBishop, ChessPieceWithColor.WhiteBishop);
                ChessPieceGraphics.DrawPiece(BlackPlayerPawn, ChessPieceWithColor.WhitePawn);
            }
        }

        public void OnGameStateChanged(GameState gameState)
        {
            if (gameState.Board != null)
            {
                for (int y = 0; y < 8; y++)
                    for (int x = 0; x < 8; x++)
                        ChessBoard[y, x] = gameState.Board[y, x];

                WhitePlayerSymbol.Opacity = gameState.WhiteMove ? 1 : 0;
                BlackPlayerSymbol.Opacity = !gameState.WhiteMove ? 1 : 0;

                ShowTime(WhitePlayerTime, WhiteClock = gameState.WhiteClock);
                ShowTime(BlackPlayerTime, BlackClock = gameState.BlackClock);

                dispatcherTimer.Start();
                lastTick = DateTime.UtcNow;
            }

            if (gameState.WhitePieces != null)
            {
                SetWhiteQueenCount(gameState.WhitePieces.Count(cp => cp == ChessPieceType.Queen));
                SetWhiteRookCount(gameState.WhitePieces.Count(cp => cp == ChessPieceType.Rook));
                SetWhiteKnightCount(gameState.WhitePieces.Count(cp => cp == ChessPieceType.Knight));
                SetWhiteBishopCount(gameState.WhitePieces.Count(cp => cp == ChessPieceType.Bishop));
                SetWhitePawnCount(gameState.WhitePieces.Count(cp => cp == ChessPieceType.Pawn));
            }

            if (gameState.BlackPieces != null)
            {
                SetBlackQueenCount(gameState.BlackPieces.Count(cp => cp == ChessPieceType.Queen));
                SetBlackRookCount(gameState.BlackPieces.Count(cp => cp == ChessPieceType.Rook));
                SetBlackKnightCount(gameState.BlackPieces.Count(cp => cp == ChessPieceType.Knight));
                SetBlackBishopCount(gameState.BlackPieces.Count(cp => cp == ChessPieceType.Bishop));
                SetBlackPawnCount(gameState.BlackPieces.Count(cp => cp == ChessPieceType.Pawn));
            }
        }

        public void OnGameEnded(GameEndedInfo info)
        {
            dispatcherTimer.Stop();
            GameResultText.Text = info.Message;
            if (info.WhitePlayerPoints > info.BlackPlayerPoints)
            {
                WhitePlayer.Foreground = new SolidColorBrush(Colors.Green);
                BlackPlayer.Foreground = new SolidColorBrush(Colors.Red);
            }
            else if (info.WhitePlayerPoints > info.BlackPlayerPoints)
            {
                WhitePlayer.Foreground = new SolidColorBrush(Colors.Red);
                BlackPlayer.Foreground = new SolidColorBrush(Colors.Green);
            }
            else
            {
                WhitePlayer.Foreground = new SolidColorBrush(Colors.Yellow);
                BlackPlayer.Foreground = new SolidColorBrush(Colors.Yellow);
            }
        }

        private void ShowTime(TextBlock text, TimeSpan time)
        {
            if (time.TotalHours >= 1)
            {
                text.Text = time.ToString(@"h\:mm\:ss");
                text.Foreground = originalTextColor;
            }
            else if (time.TotalMinutes >= 1)
            {
                text.Text = time.ToString(@"m\:ss");
                text.Foreground = originalTextColor;
            }
            else if (time.TotalSeconds > 10)
            {
                text.Text = time.ToString(@"m\:ss");
                text.Foreground = originalTextColor;
            }
            else if (time < TimeSpan.Zero)
            {
                text.Foreground = new SolidColorBrush(Colors.Red);
                text.Text = time.ToString(@"\-s\.f");
            }
            else
            {
                text.Foreground = new SolidColorBrush(Colors.Red);
                text.Text = time.ToString(@"s\.f");
            }
        }

        private void SetWhiteQueenCount(int count)
        {
            SetChessPieceCount(WhitePlayerQueen, WhitePlayerQueenCount, count);
        }

        private void SetWhiteRookCount(int count)
        {
            SetChessPieceCount(WhitePlayerRook, WhitePlayerRookCount, count);
        }

        private void SetWhiteKnightCount(int count)
        {
            SetChessPieceCount(WhitePlayerKnight, WhitePlayerKnightCount, count);
        }

        private void SetWhiteBishopCount(int count)
        {
            SetChessPieceCount(WhitePlayerBishop, WhitePlayerBishopCount, count);
        }

        private void SetWhitePawnCount(int count)
        {
            SetChessPieceCount(WhitePlayerPawn, WhitePlayerPawnCount, count);
        }

        private void SetBlackQueenCount(int count)
        {
            SetChessPieceCount(BlackPlayerQueen, BlackPlayerQueenCount, count);
        }

        private void SetBlackRookCount(int count)
        {
            SetChessPieceCount(BlackPlayerRook, BlackPlayerRookCount, count);
        }

        private void SetBlackKnightCount(int count)
        {
            SetChessPieceCount(BlackPlayerKnight, BlackPlayerKnightCount, count);
        }

        private void SetBlackBishopCount(int count)
        {
            SetChessPieceCount(BlackPlayerBishop, BlackPlayerBishopCount, count);
        }

        private void SetBlackPawnCount(int count)
        {
            SetChessPieceCount(BlackPlayerPawn, BlackPlayerPawnCount, count);
        }

        private static void SetChessPieceCount(Canvas canvas, TextBlock textCount, int count)
        {
            if (count <= 0)
            {
                canvas.Opacity = 0.2;
                textCount.Text = " ";
            }
            else if (count == 1)
            {
                canvas.Opacity = 1;
                textCount.Text = " ";
            }
            else
            {
                canvas.Opacity = 1;
                textCount.Text = count.ToString();
            }
        }
    }
}
