using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Bookstore_Tycoon.Models;
using Xamarin.Forms;
using Xamarin.Essentials;

namespace Bookstore_Tycoon.Views
{
    [QueryProperty(nameof(GameID), nameof(GameID))]
    public partial class GameplayHomePage : ContentPage
    {
        public string GameID
        {
            set
            {
                LoadGame(value);
                UpdateBindings();
            }
        }

        public GameplayHomePage()
        {
            InitializeComponent();
            // Set the BindingContext of the page to a new set of game stats.
            BindingContext = new GameData();
        }

        void LoadGame(string filename)
        {
            try
            {
                // Retrieve the note and set it as the BindingContext of the page.
                List<string> fileData = File.ReadAllLines(filename).ToList();

                if (filename.EndsWith(".gamedata.txt"))
                {
                    GameData game = new GameData
                    {
                        Filename = filename,
                        Date = File.GetCreationTime(filename),
                        // these values all come from the .gamedata.txt file
                        GameName = fileData[0],
                        GameLength = Convert.ToInt32(fileData[1]),
                        StartingCash = Convert.ToInt32(fileData[2]),
                        MoneyMultiplier = Convert.ToDouble(fileData[3]),
                        RandomEvents = Convert.ToBoolean(fileData[4]),
                        AdvertBase = Convert.ToDouble(fileData[5]),
                        CurrentCash = Convert.ToInt32(fileData[6]),
                        CurrentDebt = Convert.ToInt32(fileData[7]),
                        Markup = Convert.ToDouble(fileData[8]),
                        AdvertBonus = Convert.ToDouble(fileData[9]),
                        Interest = Convert.ToDouble(fileData[10]),
                        Inventory = Convert.ToInt32(fileData[12]),
                        UpgradeLVL = Convert.ToInt32(fileData[12]),
                        CurrentTurn = Convert.ToInt32(fileData[13]),
                        AdvertTotal = (int)Math.Floor(Convert.ToDouble(fileData[5]) + (Convert.ToDouble(fileData[5]) * Convert.ToDouble(fileData[9]))),
                        SatisfactionBonus = (Convert.ToInt32(fileData[12]) + 1) / 2 + ((Convert.ToDouble(fileData[8]) - 0.5) * -5),
                        Score = 0 // this is just as a base, score is dealt with below
                    };
                    #region Score
                    double UpgradeCost = 0;
                    for (double i = 1; i < game.UpgradeLVL; i++)
                    {
                        UpgradeCost += Math.Floor(Math.Pow(i / 2, 1.9) * 40) + 10;
                    }
                    game.Score = (int)((
                        (UpgradeCost / 2) +
                        (game.AdvertTotal * 15) +
                        (game.CurrentCash - game.CurrentDebt) +
                        (game.Inventory * 100)
                        ) / game.MoneyMultiplier);
                    #endregion
                    BindingContext = game;
                }
                else
                {
                    throw new Exception();
                }
            }
            catch (Exception)
            {
                Debug.WriteLine("Failed to load game!");
            }
        }

        void UpdateBindings()
        {
            var game = (GameData)BindingContext;
            var month = Math.Floor(game.CurrentTurn / 5.0);
            var week = game.CurrentTurn - month * 5.0;

            if (game.CurrentTurn == 0)
            {
                ScoreText.Text = "Welcome to your new bookstore! " +
                    "You have just rented a spot in a mall and are now ready to set up shop. " +
                    "Like you though, multiple other bookstore owners are also setting up in the same mall. " +
                    "You will have to use your superior business (and might I say, luck) skills to come out in the lead.";
            }
            else
            {
                ScoreText.Text = "Your score is: " + game.Score.ToString() + Environment.NewLine;
            }
            CurrentTurnText.Text = $"Your are on turn: {game.CurrentTurn}{Environment.NewLine}That is, month: {month + 1} and week: {week}";

            if (game.CurrentTurn == 0)
            {
                ContinueButton.Text = "Start your bookstore!";
            }
            else if (game.GameLength == month)
            {
                ContinueButton.Text = "The Game is over!";
            }
            else if (week == 0)
            {
                ContinueButton.Text = "Continue to monthly management";
            }
            else
            {
                ContinueButton.Text = $"Continue to week {week} of month {month + 1}";
            }
        }

        async void OnContinueButtonClicked(object sender, EventArgs e)
        {
            var game = (GameData)BindingContext;
            var month = Math.Floor(game.CurrentTurn / 5.0);
            var week = game.CurrentTurn - month * 5.0;

            if (game.CurrentTurn == 0)
            {
                await Shell.Current.GoToAsync($"{nameof(MonthlyManagementPage)}?{nameof(MonthlyManagementPage.GameID)}={game.Filename}");
            }
            else if (game.GameLength == month)
            {
                await Shell.Current.GoToAsync($"{nameof(GameStatsPage)}?{nameof(GameStatsPage.GameID)}={game.Filename}");
            }
            else if (week == 0)
            {
                await Shell.Current.GoToAsync($"{nameof(InterestChangePage)}?{nameof(InterestChangePage.GameID)}={game.Filename}");
            }
            else
            {
                await Shell.Current.GoToAsync($"{nameof(WeeklyTurnPage)}?{nameof(WeeklyTurnPage.GameID)}={game.Filename}");
            }
        }

        async void OnBackToMainMenuButtonClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync(nameof(ChooseGamePage));
        }

        async void OnDeleteButtonClicked(object sender, EventArgs e)
        {
            var game = (GameData)BindingContext;

            System.Diagnostics.Debug.WriteLine($"filename = {game.Filename}");
            // Delete the file.
            if (File.Exists(game.Filename))
            {
                File.Delete(game.Filename);
            }

            // Navigate backwards
            await Shell.Current.GoToAsync(nameof(ChooseGamePage));
        }

        async void OnGoToStatsPageClicked(object sender, EventArgs e)
        {
            var game =(GameData)BindingContext;
            await Shell.Current.GoToAsync($"{nameof(GameStatsPage)}?{nameof(GameStatsPage.GameID)}={game.Filename}");
        }

        async void OnSendFeedbackClicked(object sender, EventArgs e)
        {
            await Launcher.OpenAsync("https://docs.google.com/forms/d/1zOdPUYkvqt4dosjJ4FnvoB9NjWXDH14P_VdVnO-S0mY");
        }
    }
}