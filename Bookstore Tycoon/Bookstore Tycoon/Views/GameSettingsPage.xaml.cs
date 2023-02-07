﻿using System;
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
                            RealDice = Convert.ToBoolean(fileData[1]),
                            GameLength = Convert.ToInt32(fileData[2]),
                            StartingCash = Convert.ToInt32(fileData[3]),
                            MoneyMultiplier = Convert.ToDouble(fileData[4]),
                            RandomEvents = Convert.ToBoolean(fileData[5]),
                            AdvertBase = Convert.ToDouble(fileData[6]),
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
                            GameName = "Unnamed Game",
                            RealDice = true,
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
                "Copy this to your clipboard then press 'paste settings' in the game." + Environment.NewLine +
                game.GameName + Environment.NewLine +
                game.RealDice.ToString() + Environment.NewLine +
                game.GameLength.ToString() + Environment.NewLine +
                game.StartingCash.ToString() + Environment.NewLine +
                game.MoneyMultiplier.ToString() + Environment.NewLine +
                game.RandomEvents.ToString() + Environment.NewLine +
                game.AdvertBase.ToString();

            await Clipboard.SetTextAsync(clipboardData);
            CopyPasteStatusText.Text = "Settings copied successfully";
        }

        async void OnPasteSettings(object sender, EventArgs e)
        {
            if (Clipboard.HasText)
            {
                var game = (GameData)BindingContext;

                var clipboardText = await Clipboard.GetTextAsync();
                var clipboardData = clipboardText.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);

                if (clipboardData[0] == "Copy this to your clipboard then press 'paste settings' in the game.")
                {
                    game.GameName = clipboardData[1];
                    game.RealDice = Convert.ToBoolean(clipboardData[2]);
                    game.GameLength = Convert.ToInt32(clipboardData[3]);
                    game.StartingCash = Convert.ToInt32(clipboardData[4]);
                    game.MoneyMultiplier = Convert.ToDouble(clipboardData[5]);
                    game.RandomEvents = Convert.ToBoolean(clipboardData[6]);
                    game.AdvertBase = Convert.ToDouble(clipboardData[7]);
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
                game.RealDice.ToString(),
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

            RealDiceSwitch.IsToggled = game.RealDice;
            GameLenghText.Text = game.GameLength.ToString();
            GameLenghStepper.Value = game.GameLength;
            StartingCashText.Text = game.StartingCash.ToString();
            StartingCashStepper.Value = game.StartingCash;
            MoneyMultiplierText.Text = game.MoneyMultiplier.ToString();
            MoneyMultiplierStepper.Value = game.MoneyMultiplier;
            RandomEventsSwitch.IsToggled = game.RandomEvents;
            AdvertBaseText.Text = game.AdvertBase.ToString();
            AdvertBaseStepper.Value = game.AdvertBase;
        }
    }
}