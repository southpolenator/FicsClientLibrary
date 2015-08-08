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

        public GameControl()
        {
            dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Tick += OnTimer;
            dispatcherTimer.Interval = TimeSpan.FromMilliseconds(100);
            this.InitializeComponent();
            originalTextColor = WhitePlayer.Foreground;
        }

        public bool IsWhiteMove
        {
            get { return WhitePlayerSymbol.Visibility == Visibility.Visible; }
        }

        public bool BlackShouldBeDown
        {
            get { return ChessBoard.Flipped; }
            set { ChessBoard.Flipped = value; }
        }

        public TimeSpan WhiteClock { get; private set; }

        public TimeSpan BlackClock { get; private set; }

        private void OnTimer(object sender, object e)
        {
            if (IsWhiteMove)
            {
                WhiteClock -= dispatcherTimer.Interval;
                ShowTime(WhitePlayerTime, WhiteClock);
            }
            else
            {
                BlackClock -= dispatcherTimer.Interval;
                ShowTime(BlackPlayerTime, BlackClock);
            }
        }

        public void InitializeGameInfo(GameInfo info)
        {
            WhitePlayer.Text = info.WhitePlayer.ToString();
            BlackPlayer.Text = info.BlackPlayer.ToString();
        }

        public void OnGameStateChanged(GameState gameState)
        {
            if (gameState.Board != null)
            {
                for (int y = 0; y < 8; y++)
                    for (int x = 0; x < 8; x++)
                        ChessBoard[y, x] = gameState.Board[y, x];

                WhitePlayerSymbol.Visibility = gameState.WhiteMove ? Visibility.Visible : Visibility.Collapsed;
                BlackPlayerSymbol.Visibility = !gameState.WhiteMove ? Visibility.Visible : Visibility.Collapsed;

                ShowTime(WhitePlayerTime, WhiteClock = gameState.WhiteClock);
                ShowTime(BlackPlayerTime, BlackClock = gameState.BlackClock);
                dispatcherTimer.Start();
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
                text.Text = string.Format("{0}:{1:00}:{2:00}", time.Hours, time.Minutes, time.Seconds);
                text.Foreground = originalTextColor;
            }
            else if (time.TotalMinutes >= 1)
            {
                text.Text = string.Format("{0}:{1:00}", time.Minutes, time.Seconds);
                text.Foreground = originalTextColor;
            }
            else if (time.TotalSeconds > 10)
            {
                text.Text = string.Format("{0}:{1:00}", time.Minutes, time.Seconds);
                text.Foreground = originalTextColor;
            }
            else
            {
                text.Foreground = new SolidColorBrush(Colors.Red);
                text.Text = string.Format("{0}.{1}", time.Seconds, Math.Round(time.Milliseconds / 100.0));
            }
        }
    }
}
