using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Bookstore_Tycoon.Models;
using Xamarin.Forms;
using Xamarin.Essentials;

namespace Bookstore_Tycoon.Views
{
    [QueryProperty(nameof(ItemId), nameof(ItemId))]
    public partial class GameSettingsPage : ContentPage
    {
        public string ItemId
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
            BindingContext = new GameSettings();
        }

        void LoadGame(string filename)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(filename))
                {
                    System.Diagnostics.Debug.WriteLine("Invalid Filename!");
                }

                // Retrieve the note and set it as the BindingContext of the page.
                List<string> fileData = File.ReadAllLines(filename).ToList();
                GameSettings game = new GameSettings
                {
                    Filename = filename,
                    Date = File.GetCreationTime(filename),

                    GameName = fileData[0],
                    RealDice = Convert.ToBoolean(fileData[1]),
                    GameLength = Convert.ToInt32(fileData[2]),
                    StartingCash = Convert.ToInt32(fileData[3]),
                    MoneyMultiplier = Convert.ToDouble(fileData[4]),
                    RandomEvents = Convert.ToBoolean(fileData[5]),
                    AdvertBase = Convert.ToDouble(fileData[6])
                };

                BindingContext = game;
            }
            catch (Exception)
            {
                System.Diagnostics.Debug.WriteLine("Failed to load game!");
            }
        }

        async void OnSaveButtonClicked(object sender, EventArgs e)
        {
            var game = (GameSettings)BindingContext;

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
                game.AdvertBase.ToString()
            };
            // put our data into the file
            File.AppendAllLines(game.Filename, fileData);


            // Navigate backwards
            await Shell.Current.GoToAsync("..");
        }

        async void OnDeleteButtonClicked(object sender, EventArgs e)
        {
            var game = (GameSettings)BindingContext;

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
            var game = (GameSettings)BindingContext;
            await Clipboard.SetTextAsync(File.ReadAllText(game.Filename));
        }

        async void OnPasteSettings(object sender, EventArgs e)
        {
            if (Clipboard.HasText)
            {
                var game = (GameSettings)BindingContext;

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

        void OnUpdateBindings(object sender, EventArgs e)
        {
            UpdateBindings();
        }

        void UpdateBindings()
        {
            var game = (GameSettings)BindingContext;

            RealDiceSwitch.IsToggled = game.RealDice;
            GameLenghText.Text = game.GameLength.ToString();
            StartingCashText.Text = game.StartingCash.ToString();
            MoneyMultiplierText.Text = game.MoneyMultiplier.ToString();
            RandomEventsSwitch.IsToggled = game.RandomEvents;
            AdvertBaseText.Text = game.AdvertBase.ToString();
        }
    }
}