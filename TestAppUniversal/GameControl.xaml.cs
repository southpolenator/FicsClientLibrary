using Internet.Chess.Server.Fics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
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
            foreach (ChessPieceColor pieceColor in Enum.GetValues(typeof(ChessPieceColor)))
                foreach (ChessPieceType pieceType in Enum.GetValues(typeof(ChessPieceType)))
                    SetChessPieceCount(pieceColor, pieceType, 0);
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
            foreach (ChessPieceColor pieceColor in Enum.GetValues(typeof(ChessPieceColor)))
                foreach (ChessPieceType pieceType in Enum.GetValues(typeof(ChessPieceType)))
                {
                    ChessPieceWithColor chessPiece = new ChessPieceWithColor()
                    {
                        Color = !inverted ? pieceColor : (pieceColor == ChessPieceColor.Black ? ChessPieceColor.White : ChessPieceColor.Black),
                        Type = pieceType,
                    };
                    string canvasPropertyName = string.Format("{0}Player{1}", pieceColor, pieceType);
                    var canvasProperty = GetType().GetTypeInfo().GetDeclaredProperty(canvasPropertyName);

                    if (canvasProperty != null)
                        ChessPieceGraphics.DrawPiece((Canvas)canvasProperty.GetValue(this), chessPiece);
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
                foreach (ChessPieceType pieceType in Enum.GetValues(typeof(ChessPieceType)))
                {
                    SetChessPieceCount(ChessPieceColor.White, pieceType, gameState.WhitePieces.Count(cp => cp == pieceType));
                }
            }

            if (gameState.BlackPieces != null)
            {
                foreach (ChessPieceType pieceType in Enum.GetValues(typeof(ChessPieceType)))
                {
                    SetChessPieceCount(ChessPieceColor.Black, pieceType, gameState.BlackPieces.Count(cp => cp == pieceType));
                }
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

        private void SetChessPieceCount(ChessPieceColor color, ChessPieceType type, int count)
        {
            string canvasPropertyName = string.Format("{0}Player{1}", color, type);
            string textCountPropertyName = string.Format("{0}Player{1}Count", color, type);
            var canvasProperty = GetType().GetTypeInfo().GetDeclaredProperty(canvasPropertyName);
            var textCountcanvasProperty = GetType().GetTypeInfo().GetDeclaredProperty(textCountPropertyName);

            if (canvasProperty != null && textCountcanvasProperty != null)
                SetChessPieceCount((Canvas)canvasProperty.GetValue(this), (TextBlock)textCountcanvasProperty.GetValue(this), count);
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
