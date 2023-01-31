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
    public partial class MonthlyManagementPage : ContentPage
    {
        public string GameID
        {
            set
            {
                LoadGame(value);
                UpdateBindings();
            }
        }

        public MonthlyManagementPage()
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

                    // set the minimums for the steppers
                    UpgradeLVLStepper.Minimum = game.UpgradeLVL;
                    AdvertisingStepper.Minimum = game.AdvertBonus;
                    InventoryStepper.Minimum = game.Inventory;
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

        async void OnContinueButtonClicked(object sender, EventArgs e)
        {
            var game = (GameData)BindingContext;

            List<string> fileData = File.ReadAllLines(game.Filename).ToList();
            GameData gameOnFile = new GameData
            {
                CurrentCash = Convert.ToInt32(fileData[7]),
                CurrentDebt = Convert.ToInt32(fileData[8]),
                Markup = Convert.ToDouble(fileData[9]),
                AdvertBonus = Convert.ToDouble(fileData[10]),
                Interest = Convert.ToDouble(fileData[11]),
                Inventory = Convert.ToInt32(fileData[12]),
                UpgradeLVL = Convert.ToInt32(fileData[13]),
                CurrentTurn = Convert.ToInt32(fileData[14])
            };

            // Here I have to do the computations

            int InterestCost = (int)(game.Interest * game.CurrentDebt);
            int UpkeepCost = (gameOnFile.UpgradeLVL ^ 2) + 25;
            int AdvertisingCost = (int)Math.Pow(((game.AdvertBonus * 100) + 5) / 7, 3);
            int InventoryCost = (game.Inventory - gameOnFile.Inventory) * 200;
            #region int UpgradeCost = for (more info) { see inside }
            double UpgradeCost = 0;
            for (double i = gameOnFile.UpgradeLVL; i < game.UpgradeLVL; i++)
            {
                UpgradeCost += Math.Floor(Math.Pow(i / 2, 1.9) * 40) + 10;
            }
            #endregion
            int TotalCost = InterestCost + UpkeepCost + AdvertisingCost + InventoryCost + (int)UpgradeCost;
            game.CurrentCash -= TotalCost;


            // We delete the file to clear it then make a new one with the same name
            if (File.Exists(game.Filename))
            {
                File.Delete(game.Filename);
            }

            // Here's the data we will write to the file
            var newFileData = new List<string>
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
                (game.CurrentTurn + 1).ToString()
            };

            // Put our data into the file
            File.AppendAllLines(game.Filename, newFileData);

            // Navigate backwards
            await Shell.Current.GoToAsync($"{nameof(GameplayHomePage)}?{nameof(GameplayHomePage.GameID)}={game.Filename}");
        }

        async void OnGoToStatsPageClicked(object sender, EventArgs e)
        {
            var game = (GameData)BindingContext;
            await Shell.Current.GoToAsync($"{nameof(GameStatsPage)}?{nameof(GameStatsPage.GameID)}={game.Filename}");
        }

        void OnUpdateBindings(object sender, EventArgs e)
        {
            UpdateBindings();
        }

        void UpdateBindings()
        {
            var game = (GameData)BindingContext;

            List<string> fileData = File.ReadAllLines(game.Filename).ToList();
            GameData gameOnFile = new GameData
            {
                // these values all come from the .gamedata.txt file
                CurrentCash = Convert.ToInt32(fileData[7]),
                CurrentDebt = Convert.ToInt32(fileData[8]),
                Markup = Convert.ToDouble(fileData[9]),
                AdvertBonus = Convert.ToDouble(fileData[10]),
                Interest = Convert.ToDouble(fileData[11]),
                Inventory = Convert.ToInt32(fileData[12]),
                UpgradeLVL = Convert.ToInt32(fileData[13]),
                CurrentTurn = Convert.ToInt32(fileData[14]),
            };

            // Here I have to do the computations (I'll have to copy this over to the saving function too)

            int InterestCost = (int)(game.Interest * game.CurrentDebt);
            int UpkeepCost = (gameOnFile.UpgradeLVL ^ 2) + 25;
            int AdvertisingCost = (int)Math.Pow(((game.AdvertBonus * 100) + 5) / 7, 3);
            int InventoryCost = (game.Inventory - gameOnFile.Inventory) * 200;
            #region int UpgradeCost = for (more info) { see inside }
            double UpgradeCost = 0;
            for (double i = gameOnFile.UpgradeLVL; i < game.UpgradeLVL; i++)
            {
                UpgradeCost += Math.Floor(Math.Pow(i / 2, 1.9) * 40) + 10;
            }
            #endregion
            int TotalCost = InterestCost + UpkeepCost + AdvertisingCost + InventoryCost + (int)UpgradeCost;


            MarkupText.Text = "Satisfaction Bonus: " + ((game.Markup - 0.5) * -5).ToString()
                + Environment.NewLine + (game.Markup * 100).ToString() + "%";
            UpgradeCostText.Text = "Satisfaction Bonus: " + ( (game.UpgradeLVL + 1.0) / 2.0).ToString()
                + Environment.NewLine + "Upgrade Cost: $" + UpgradeCost.ToString() + "$" // Need to make this work (data from Excel)
                + Environment.NewLine + "Upkeep Cost: $" + UpkeepCost.ToString() + "$";
            UpgradeLVLText.Text = game.UpgradeLVL.ToString();
            AdvertisingText.Text = "Advert. Bonus Increase: " + ((game.AdvertBonus - gameOnFile.AdvertBonus) * 100).ToString() + "%"
                + Environment.NewLine + "Cost: $" + AdvertisingCost;
            InventoryText.Text = "Inventory: " + game.Inventory.ToString()
                + Environment.NewLine + "Cost: $" + InventoryCost;

            ExpensesText.Text = $"Interest({game.Interest * 100}%): $" + InterestCost.ToString()
                + Environment.NewLine + "Upkeep: $" + UpkeepCost.ToString() + "$"
                + Environment.NewLine + "Advertising: $" + AdvertisingCost.ToString()
                + Environment.NewLine + "Inventory: $" + InventoryCost.ToString()
                + Environment.NewLine + "Upgrades: $" + UpgradeCost.ToString()
                + Environment.NewLine + "Towards Debts: " + "<Feature Unavilable!>"
                + Environment.NewLine + "Total Expenses: $" + TotalCost.ToString()
                + Environment.NewLine + "Current Cash: $" + gameOnFile.CurrentCash.ToString()
                + Environment.NewLine + "New Total Cash: $" + (gameOnFile.CurrentCash - TotalCost).ToString();
        }
    }
}