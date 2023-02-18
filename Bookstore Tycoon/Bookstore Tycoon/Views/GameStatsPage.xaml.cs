using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Bookstore_Tycoon.Models;
using Xamarin.Forms;
using System.Diagnostics;

namespace Bookstore_Tycoon.Views
{
    [QueryProperty(nameof(GameID), nameof(GameID))]
    public partial class GameStatsPage : ContentPage
    {
        public string GameID
        {
            set
            {
                LoadGame(value);
                UpdateBindings();
            }
        }

        public GameStatsPage()
        {
            InitializeComponent();
            // Set the BindingContext of the page to a new game data set.
            BindingContext = new GameData();
        }

        void LoadGame(string filename)
        {
            try
            {
                // Retrieve the game data and set it as the BindingContext of the page.
                List<string> fileData = File.ReadAllLines(filename).ToList();

                if(filename.EndsWith(".gamedata.txt"))
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
                        Score = (int)( Convert.ToInt32(fileData[7]) / Convert.ToDouble(fileData[4])
                        + Convert.ToDouble(fileData[10]) * 200
                        - Convert.ToInt32(fileData[8]) * 1.1)
                    };
                    #region Score += UpgradeCost
                    for (double i = game.UpgradeLVL; i < game.UpgradeLVL; i++)
                    {
                        game.Score += (int)(Math.Floor(Math.Pow(i / 2, 1.9) * 40) + 10);
                    }
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

        async void OnBackButtonClicked(object sender, EventArgs e)
        {
            // Navigate backwards
            await Navigation.PopAsync();
        }

        void UpdateBindings()
        {
            var game = (GameData)BindingContext;

            GameNameText.Text = "Game Name: " + game.GameName;
            GameLengthText.Text = "Game Length: " + game.GameLength + " months";
            StartingCashText.Text = "The cash you started with: $" + game.StartingCash;
            MoneyMultiplierText.Text = "Money Multiplier: " + game.MoneyMultiplier + "x";
            RandomEventsText.Text = "Random Events: " + game.RandomEvents;
            AdvertBaseText.Text = "Advertisment Base: " + game.AdvertBase;
            CurrentCashText.Text = "Current Cash: $" + game.CurrentCash;
            CurrentDebtText.Text = "Current Debt: $" + game.CurrentDebt;
            MarkupText.Text = "Markup: " + (game.Markup*100) + "%";
            AdvertBonusText.Text = "Advertisment Bonus: " + (game.AdvertBonus * 100) + "%";
            InterestText.Text = "Current Interest: " + (game.Interest * 100) + "%";
            InventoryText.Text = "Inventory: " + game.Inventory;
            UpgradeLVLText.Text = "Upgrade Level: " + game.UpgradeLVL;
            CurrentTurnText.Text = "Current Turn: " + game.CurrentTurn;
            AdvertTotalText.Text = "Advertisment Total: " + game.AdvertTotal;
            SatisfactionBonusText.Text = "Satisfaction: " + game.SatisfactionBonus;
            ScoreText.Text = "Your Score: " + game.Score;
        }
    }
}
