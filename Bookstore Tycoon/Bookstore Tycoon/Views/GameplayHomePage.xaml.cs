using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Bookstore_Tycoon.Models;
using Xamarin.Forms;

namespace Bookstore_Tycoon.Views
{
    [QueryProperty(nameof(GameID), nameof(GameID))]
    public partial class GameplayHomePage : ContentPage
    {
        private bool MonthlyManagement = false;

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

                if(filename.EndsWith(".gamedata.txt"))
                {
                    GameData game = new GameData
                    {
                        Filename = filename,
                        Date = File.GetCreationTime(filename),
                        GameName = fileData[0],
                        RealDice = Convert.ToBoolean(fileData[1]),
                        GameLength = Convert.ToInt32(fileData[2]),
                        StartingCash = Convert.ToInt32(fileData[3]),
                        MoneyMultiplier = Convert.ToDouble(fileData[4]),
                        RandomEvents = Convert.ToBoolean(fileData[5]),
                        AdvertBase = Convert.ToDouble(fileData[6]),
                        CurrentCash = Convert.ToInt32(fileData[7]),
                        CurrentDebt = Convert.ToInt32(fileData[8]),
                        Markup = Convert.ToDouble(fileData[9]),
                        AdvertBonus = Convert.ToDouble(fileData[10]),
                        Interest = Convert.ToDouble(fileData[11]),
                        Inventory = Convert.ToInt32(fileData[12]),
                        UpgradeLVL = Convert.ToInt32(fileData[13]),
                        CurrentTurn = Convert.ToInt32(fileData[14])
                    };
                    BindingContext = game;
                }
                else
                {
                    throw new Exception();
                }
            }
            catch (Exception)
            {
                System.Diagnostics.Debug.WriteLine("Failed to load game!");
            }
        }

        void UpdateBindings()
        {
            var game = (GameData)BindingContext;
            var month = Math.Floor(game.CurrentTurn / 5.0);
            var week = game.CurrentTurn - month * 5.0;

            ScoreText.Text = "Your score is: " + game.Score.ToString() + Environment.NewLine;
            CurrentTurnText.Text = $"Your are on turn: {game.CurrentTurn}{Environment.NewLine}That is, month: {month} and week: {week}";

            if (week == 0)
            {
                ContinueButton.Text = "Continue to monthly management" + Environment.NewLine;
                MonthlyManagement = true;
            }
            else
            {
                ContinueButton.Text = $"Continue to week #{week} of month #{month}" + Environment.NewLine;
                MonthlyManagement = false;
            }
        }

        async void OnContinueButtonClicked(object sender, EventArgs e)
        {
            var game = (GameData)BindingContext;
            if (MonthlyManagement == true)
            {
                if (game.CurrentTurn == 0)
                {
                    await Shell.Current.GoToAsync($"{nameof(MonthlyManagementPage)}?{nameof(MonthlyManagementPage.GameID)}={game.Filename}");
                }
                else
                {
                    await Shell.Current.GoToAsync($"{nameof(InterestChangePage)}?{nameof(InterestChangePage.GameID)}={game.Filename}");
                }
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
    }
}