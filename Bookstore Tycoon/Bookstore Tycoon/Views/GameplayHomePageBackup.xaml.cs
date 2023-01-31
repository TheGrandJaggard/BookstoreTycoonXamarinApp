﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Bookstore_Tycoon.Models;
using Xamarin.Forms;
using Xamarin.Essentials;

namespace Bookstore_Tycoon.Views
{
    [QueryProperty(nameof(GameID), nameof(GameID))]
    public partial class GameplayHomePageBackup : ContentPage
    {
        public string GameID
        {
            set
            {
                LoadGame(value);
                UpdateBindings();
            }
        }

        public GameplayHomePageBackup()
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
                        // these values all come from the .gamedata.txt file
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

        /*async*/ void OnSaveButtonClicked(object sender, EventArgs e)
        {
            var game = (GameData)BindingContext;

            // we delete the file to clear it then make a new one with the same name
            if (File.Exists(game.Filename))
            {
                File.Delete(game.Filename);
            }

            // here's the data we will write to the file
            var fileData = new List<string>
            {
                game.GameName,
                game.RealDice.ToString(),
                game.GameLength.ToString(),
                game.StartingCash.ToString(),
                game.MoneyMultiplier.ToString(),
                game.RandomEvents.ToString(),
                game.AdvertBase.ToString(),
                game.CurrentCash.ToString(),
                game.CurrentDebt.ToString(),
                game.Markup.ToString(),
                game.AdvertBonus.ToString(),
                game.Interest.ToString(),
                game.Inventory.ToString(),
                game.UpgradeLVL.ToString(),
                game.CurrentTurn.ToString()
            };
            // put our data into the file
            File.AppendAllLines(game.Filename, fileData);


            // Navigate backwards
            //await Shell.Current.GoToAsync($"{nameof(ChooseGamePage)}");
        }

        void OnUpdateBindings(object sender, EventArgs e)
        {
            UpdateBindings();
        }

        void UpdateBindings()
        {
            var game = (GameData)BindingContext;

            CurrentCashText.Text = game.CurrentCash.ToString();
            CurrentDebtText.Text = game.CurrentDebt.ToString();
            MarkupText.Text = game.Markup.ToString();
            AdvertBonusText.Text = game.AdvertBonus.ToString();
            InterestText.Text = game.Interest.ToString();
            InventoryText.Text = game.Inventory.ToString();
            UpgradeLVLText.Text = game.UpgradeLVL.ToString();
        }

        async void OnQuitButtonClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync($"{nameof(ChooseGamePage)}");
        }

        // unused code here
        /*
        async void OnDeleteButtonClicked(object sender, EventArgs e)
        {
            var game = (GameStats)BindingContext;

            // Delete the file.
            if (File.Exists(game.Filename))
            {
                File.Delete(game.Filename);
            }

            // Navigate backwards
            await Shell.Current.GoToAsync("..");
        }

        async void OnCopySettings(object sender, EventArgs e)
        {
            var game = (GameStats)BindingContext;
            await Clipboard.SetTextAsync(File.ReadAllText(game.Filename));
        }

        async void OnPasteSettings(object sender, EventArgs e)
        {
            if (Clipboard.HasText)
            {
                var game = (GameStats)BindingContext;

                var clipboardText = await Clipboard.GetTextAsync();
                var clipboardData = clipboardText.Split(new string[] { System.Environment.NewLine }, StringSplitOptions.None);

                game.RealDice = Convert.ToBoolean(clipboardData[1]);
                game.GameLength = Convert.ToInt32(clipboardData[2]);
                game.StartingCash = Convert.ToInt32(clipboardData[3]);
                game.MoneyMultiplier = Convert.ToDouble(clipboardData[4]);
                game.RandomEvents = Convert.ToBoolean(clipboardData[5]);
                game.AdvertBase = Convert.ToDouble(clipboardData[6]);
                UpdateBindings();
            }
        }
        */
    }
}