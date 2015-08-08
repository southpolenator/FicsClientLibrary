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
