using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Bookstore_Tycoon.Models;
using Xamarin.Forms;

namespace Bookstore_Tycoon.Views
{
    [QueryProperty(nameof(GameID), nameof(GameID))]
    public partial class InterestChangePage : ContentPage
    {
        public string GameID
        {
            set
            {
                LoadGame(value);
            }
        }

        public InterestChangePage()
        {
            InitializeComponent();
            // Set the BindingContext of the page to a new set of game stats.
            BindingContext = new GameData();
            // Hide the back button in the toolbar (It don't work :()
            // NavigationPage.SetHasBackButton(this, false);
        }

        void LoadGame(string filename)
        {
            try
            {
                if (filename.EndsWith(".gamedata.txt"))
                {
                    List<string> fileData = File.ReadAllLines(filename).ToList();
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
                        OtherBinding1 = 2
                    };
                    BindingContext = game;
                    UpdateBindings();
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

        async void OnContinueButtonClicked(object sender, EventArgs e)
        {
            var game = (GameData)BindingContext;

            switch (game.OtherBinding1)
            {
                case 1:
                    game.OtherBinding2 = -1;
                    break;
                case 2:
                    game.OtherBinding2 = 0;
                    break;
                case 3:
                    game.OtherBinding2 = 1;
                    break;
                case 4:
                    game.OtherBinding2 = 1;
                    break;
                case 5:
                    game.OtherBinding2 = 2;
                    break;
                case 6:
                    game.OtherBinding2 = 2;
                    break;
                default:
                    InterestText.Text = "Stepper not in range!";
                    break;
            }

            // we delete the file to clear it then make a new one with the same name
            if (File.Exists(game.Filename))
            {
                File.Delete(game.Filename);
            }

            // here's the data we will write to the file
            var fileData = new List<string>
            {
                game.GameName,
                game.GameLength.ToString(),
                game.StartingCash.ToString(),
                game.MoneyMultiplier.ToString(),
                game.RandomEvents.ToString(),
                game.AdvertBase.ToString(),
                game.CurrentCash.ToString(),
                game.CurrentDebt.ToString(),
                game.Markup.ToString(),
                game.AdvertBonus.ToString(),
                (game.Interest + game.OtherBinding2 / 100.0).ToString(),
                game.Inventory.ToString(),
                game.UpgradeLVL.ToString(),
                game.CurrentTurn.ToString()
            };

            // put our data into the file
            File.AppendAllLines(game.Filename, fileData);


            // Navigate
            await Shell.Current.GoToAsync($"{nameof(MonthlyManagementPage)}?{nameof(MonthlyManagementPage.GameID)}={game.Filename}");
        }

        async void OnGoToStatsPageClicked(object sender, EventArgs e)
        {
            GameData game = (GameData)BindingContext;
            await Shell.Current.GoToAsync($"{nameof(GameStatsPage)}?{nameof(GameStatsPage.GameID)}={game.Filename}");
        }

        void OnUpdateBindings(object sender, EventArgs e)
        {
            UpdateBindings();
        }

        void OnInterestRollClicked(object sender, EventArgs e)
        {
            GameData game = (GameData)BindingContext;
            game.OtherBinding1 = new Random().Next(1, 7);

            UpdateBindings();
        }

        void UpdateBindings()
        {
            var game = (GameData)BindingContext;

            switch (game.OtherBinding1)
            {
                case 1:
                    InterestText.Text = "Roll of " + game.OtherBinding1 + " = interest -1%";
                    break;
                case 2:
                    InterestText.Text = "Roll of " + game.OtherBinding1 + " = interest 0%";
                    break;
                case 3:
                    InterestText.Text = "Roll of " + game.OtherBinding1 + " = interest 1%";
                    break;
                case 4:
                    InterestText.Text = "Roll of " + game.OtherBinding1 + " = interest 1%";
                    break;
                case 5:
                    InterestText.Text = "Roll of " + game.OtherBinding1 + " = interest 2%";
                    break;
                case 6:
                    InterestText.Text = "Roll of " + game.OtherBinding1 + " = interest 2%";
                    break;
                default:
                    InterestText.Text = "Stepper not in range!";
                    break;
            }

            InterestDescriptionText.Text = game.CurrentTurn == 5
                ? "Every monthly management, except for the setup, starts with a roll that determines whether interest increases or deacreases this month. " +
                    "A roll of one decreases interest by one percent, a two does nothing, " +
                    "both a three and four increase it by one percent, and a five or six increase it twice that much."
                : "What did you roll for interest this turn?";
        }
    }
}