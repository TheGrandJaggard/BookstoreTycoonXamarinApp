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
                        GameName = fileData[0],
                        GameLength = Convert.ToInt32(fileData[1]),
                        CurrentTurn = Convert.ToInt32(fileData[13]),
                        Score = (int)(Convert.ToInt32(fileData[8]) / Convert.ToDouble(fileData[4])
                        + Convert.ToDouble(fileData[9]) * 200
                        - Convert.ToInt32(fileData[7]) * 1.1)
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
                ContinueButton.Text = $"Continue to week #{week} of month #{month + 1}";
            }
        }

        async void OnContinueButtonClicked(object sender, EventArgs e)
        {
            var game = (GameData)BindingContext;
            var month = Math.Floor(game.CurrentTurn / 5.0) + 1;
            var week = game.CurrentTurn - month * 5.0;

            if (game.CurrentTurn == 0)
            {
                await Shell.Current.GoToAsync($"{nameof(MonthlyManagementPage)}?{nameof(MonthlyManagementPage.GameID)}={game.Filename}");
            }
            else if (game.GameLength == month)
            {
                await Shell.Current.GoToAsync($"{nameof(InterestChangePage)}?{nameof(InterestChangePage.GameID)}={game.Filename}");
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
    }
}