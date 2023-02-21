using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Bookstore_Tycoon.Models;
using Xamarin.Forms;
using Xamarin.Essentials;
using System.Diagnostics;

namespace Bookstore_Tycoon.Views
{
    [QueryProperty(nameof(GameID), nameof(GameID))]
    public partial class GameSettingsPage : ContentPage
    {
        public string GameID
        {
            set
            {
                LoadGame(value);
                UpdateBindings();
            }
        }

        public GameSettingsPage()
        {
            InitializeComponent();

            // Set the BindingContext of the page to a new Note.
            BindingContext = new GameData();
        }

        void LoadGame(string filename)
        {
            // Debug.WriteLine($"filename = {filename}");
            try
            {
                if (filename.EndsWith(".gamedata.txt"))
                {
                    if (File.Exists(filename))
                    {
                        // Retrieve the game data
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
                        };
                        BindingContext = game;
                    }
                    else
                    {
                        // write in the defaults
                        GameData game = new GameData
                        {
                            Filename = filename,
                            // we are creating these values from scratch
                            Date = DateTime.Now,
                            GameName = "My New Game",
                            GameLength = 6,
                            StartingCash = 500,
                            MoneyMultiplier = 1.0,
                            RandomEvents = false,
                            AdvertBase = 5,
                        };
                        BindingContext = game;

                        // create the file and save the defaults
                        SaveGame();
                    }
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

        async void OnCancelButtonClicked(object sender, EventArgs e)
        {
            var game = (GameData)BindingContext;

            // Delete the file.
            if (File.Exists(game.Filename))
            {
                File.Delete(game.Filename);
            }

            // Go back to the ChooseGamePage
            await Shell.Current.GoToAsync("ChooseGamePage");
        }

        async void OnContinueButtonClicked(object sender, EventArgs e)
        {
            var game = (GameData)BindingContext;
            SaveGame();
            await Shell.Current.GoToAsync($"{nameof(GameplayHomePage)}?{nameof(GameplayHomePage.GameID)}={game.Filename}");
        }

        async void OnCopySettings(object sender, EventArgs e)
        {
            var game = (GameData)BindingContext;

            // Here is the data we will put onto the clipboard
            var clipboardData =
                "Copy this to your clipboard then press 'paste and continue' in the game." + Environment.NewLine +
                game.GameName + Environment.NewLine +
                game.GameLength.ToString() + Environment.NewLine +
                game.StartingCash.ToString() + Environment.NewLine +
                game.MoneyMultiplier.ToString() + Environment.NewLine +
                game.RandomEvents.ToString() + Environment.NewLine +
                game.AdvertBase.ToString();

            await Clipboard.SetTextAsync(clipboardData);
            CopyPasteStatusText.Text = "Settings copied successfully";

            SaveGame();
            await Shell.Current.GoToAsync($"{nameof(GameplayHomePage)}?{nameof(GameplayHomePage.GameID)}={game.Filename}");
        }

        async void OnPasteSettings(object sender, EventArgs e)
        {
            if (Clipboard.HasText)
            {
                var game = (GameData)BindingContext;

                var clipboardText = await Clipboard.GetTextAsync();
                var clipboardData = clipboardText.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);

                if (clipboardData[0] == "Copy this to your clipboard then press 'paste and continue' in the game.")
                {
                    game.GameName = clipboardData[1];
                    game.GameLength = Convert.ToInt32(clipboardData[2]);
                    game.StartingCash = Convert.ToInt32(clipboardData[3]);
                    game.MoneyMultiplier = Convert.ToDouble(clipboardData[4]);
                    game.RandomEvents = Convert.ToBoolean(clipboardData[5]);
                    game.AdvertBase = Convert.ToDouble(clipboardData[6]);
                    CopyPasteStatusText.Text = "Settings pasted successfully";
                }

                SaveGame();
                await Shell.Current.GoToAsync($"{nameof(GameplayHomePage)}?{nameof(GameplayHomePage.GameID)}={game.Filename}");
            }
        }

        void OnUpdateBindings(object sender, EventArgs e)
        {
            UpdateBindings();
        }

        private void SaveGame()
        {
            var game = (GameData)BindingContext;

            // we delete the file to clear it but will recreate it in the future
            if (File.Exists(game.Filename))
            {
                File.Delete(game.Filename);
            }

            // here's the data we will write to the file
            var fileData = new List<string>
            {
                // our settings
                game.GameName,
                game.GameLength.ToString(),
                game.StartingCash.ToString(),
                game.MoneyMultiplier.ToString(),
                game.RandomEvents.ToString(),
                game.AdvertBase.ToString(),
                // Defaults
                game.StartingCash.ToString(),   // Current Cash
                game.StartingCash.ToString(),   // Current Debt
                "0.50", // Markup
                "0.00", // AdvertBonus
                "0.05", // Interest
                "0",    // Inventory
                "1",    // UpgradeLVL
                "0"     // CurrentTurn
            };


            // put our data into the file
            File.AppendAllLines(game.Filename, fileData);
        }

        void UpdateBindings()
        {
            var game = (GameData)BindingContext;

            // RealDiceSwitch.IsToggled = game.RealDice;
            GameLenghText.Text = "Length of your game: " + game.GameLength + " months";
            GameLenghStepper.Value = game.GameLength;
            StartingCashText.Text = "Starting Cash: $" + game.StartingCash;
            StartingCashStepper.Value = game.StartingCash;
            MoneyMultiplierText.Text = "Money Multiplyer: " + game.MoneyMultiplier + "x";
            MoneyMultiplierStepper.Value = game.MoneyMultiplier;
            RandomEventsText.Text = "If you would like random events in your game turn this on";
            RandomEventsSwitch.IsToggled = game.RandomEvents;
            AdvertBaseText.Text = "Advertisment Base Value: " + game.AdvertBase;
            AdvertBaseStepper.Value = game.AdvertBase;
        }
    }
}