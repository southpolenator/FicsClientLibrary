﻿using Internet.Chess.Server.Fics;
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
        private Game leftGame;
        private Game rightGame;

        public GamePage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            leftGame = (Game)e.Parameter;
            App.Current.FicsClientReady += OnFicsClientReady;
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            if (fics != null)
            {
                fics.GameStateChange -= OnGameStateChanged;
                fics.GameEnded -= OnGameEnded;
                fics.FollowedPlayerStartedGame -= OnGameStarted;
                var t = fics.StopFollowingPlayer();
                var t2 = fics.StopObservingGame(leftGame);
                if (rightGame != null)
                {
                    var t3 = fics.StopObservingGame(rightGame);
                }
            }
        }

        private void OnFicsClientReady(FicsClient client)
        {
            App.Current.FicsClientReady -= OnFicsClientReady;
            fics = client;
            fics.GameStateChange += OnGameStateChanged;
            fics.GameEnded += OnGameEnded;
            fics.FollowedPlayerStartedGame += OnGameStarted;
            Task.Run(() => { StartGame(); });
        }

        private async void StartGame()
        {
            try
            {
                var result = await fics.StartFollowingPlayer(leftGame.WhitePlayer);

                if (result == null)
                {
                    await fics.StopFollowingPlayer();
                    result = await fics.StartFollowingPlayer(leftGame.BlackPlayer);
                }
                if (result != null)
                {
                    OnGameStarted(result);
                }
            }
            catch (Exception ex)
            {
                // TODO: Notify user that game is not playing
            }
        }

        private async void OnGameStarted(ObserveGameResult result)
        {
            if (result.GameInfo.GameId != leftGame.Id)
                leftGame = await fics.GetGame(result.GameInfo.GameId);

            var t = Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                LeftGame.InitializeGameInfo(result.GameInfo);
            });

            OnGameStateChanged(result.GameState);

            // Check if we have one more game to observe at the same time
            if (result.GameInfo.PartnersGameId > 0)
            {
                rightGame = await fics.GetGame(result.GameInfo.PartnersGameId);
                var resultRight = await fics.StartObservingGame(rightGame);

                if (resultRight.GameInfo.PartnersGameId != leftGame.Id)
                {
                    // TODO: Partners game finished (almost zero probability of this to happen)
                }

                var t2 = Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    RightGame.Visibility = Visibility.Visible;
                    RightGame.InitializeGameInfo(resultRight.GameInfo);
                });

                OnGameStateChanged(resultRight.GameState);
            }
            else
            {
                var t2 = Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    RightGame.Visibility = Visibility.Collapsed;
                });
            }
        }

        private void OnGameStateChanged(GameState gameState)
        {
            if (leftGame != null && gameState.GameId == leftGame.Id)
            {
                var t = Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    LeftGame.OnGameStateChanged(gameState);
                });
            }
            else if (rightGame != null && gameState.GameId == rightGame.Id)
            {
                var t = Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    RightGame.OnGameStateChanged(gameState);
                });
            }
        }

        private void OnGameEnded(GameEndedInfo info)
        {
            // Mark winner
            if (leftGame != null && info.GameId == leftGame.Id)
            {
                var t = Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    LeftGame.OnGameEnded(info);
                });
            }
            else if (rightGame != null && info.GameId == rightGame.Id)
            {
                var t = Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    RightGame.OnGameEnded(info);
                });
            }
        }
    }
}
