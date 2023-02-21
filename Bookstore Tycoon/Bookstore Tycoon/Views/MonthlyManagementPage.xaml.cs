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
                    };
                    BindingContext = game;

                    // set the minimums for the steppers
                    UpgradeLVLStepper.Minimum = game.UpgradeLVL;
                    AdvertisingStepper.Minimum = game.AdvertBonus;
                    InventoryStepper.Minimum = game.Inventory;
                    if (game.CurrentTurn == 0)
                    {
                        DebtStepper.Minimum = -0.1;
                        DebtStepper.Maximum = 0.1;
                    }
                    else
                    {
                        DebtStepper.Minimum = -game.CurrentDebt;
                        DebtStepper.Maximum = game.CurrentCash;
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

        async void OnContinueButtonClicked(object sender, EventArgs e)
        {
            var game = (GameData)BindingContext;

            List<string> fileData = File.ReadAllLines(game.Filename).ToList();
            GameData gameOnFile = new GameData
            {
                CurrentCash = Convert.ToInt32(fileData[6]),
                CurrentDebt = Convert.ToInt32(fileData[7]),
                Markup = Convert.ToDouble(fileData[8]),
                AdvertBonus = Convert.ToDouble(fileData[9]),
                Interest = Convert.ToDouble(fileData[10]),
                Inventory = Convert.ToInt32(fileData[11]),
                UpgradeLVL = Convert.ToInt32(fileData[12]),
                CurrentTurn = Convert.ToInt32(fileData[13])
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
            int DebtCost = -game.OtherBinding1;
            int TotalCost = InterestCost + UpkeepCost + AdvertisingCost + InventoryCost + (int)UpgradeCost + DebtCost;
            game.CurrentCash -= TotalCost;



            if (game.CurrentCash < 0)
            {
                ErrorText.Text = "Hey, you can't nave a negative amount of money!";
            }
            else
            {
                // We delete the file to clear it then make a new one with the same name
                if (File.Exists(game.Filename))
                {
                    File.Delete(game.Filename);
                }

                // Here's the data we will write to the file
                var newFileData = new List<string>
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
                CurrentCash = Convert.ToInt32(fileData[6]),
                CurrentDebt = Convert.ToInt32(fileData[7]),
                Markup = Convert.ToDouble(fileData[8]),
                AdvertBonus = Convert.ToDouble(fileData[9]),
                Interest = Convert.ToDouble(fileData[10]),
                Inventory = Convert.ToInt32(fileData[11]),
                UpgradeLVL = Convert.ToInt32(fileData[12]),
                CurrentTurn = Convert.ToInt32(fileData[13]),
            };

            // Here I have to do the computations

            int InterestCost = (int)(game.Interest * game.CurrentDebt);
            int UpkeepCost = (gameOnFile.UpgradeLVL ^ 2) + 25;
            int AdvertisingCost = (int)Math.Pow(((game.AdvertBonus * 100) + 5) / 7, 3);
            int InventoryCost = (game.Inventory - gameOnFile.Inventory) * 200;
            #region int UpgradeCost = for (more info) { see inside }
            double UpgradeCost = 0;
            for (int i = gameOnFile.UpgradeLVL; i < game.UpgradeLVL; i++)
            {
                UpgradeCost += Math.Floor(Math.Pow((double)i / 2, 1.9) * 40) + 10;
            }
            #endregion
            int DebtCost = -game.OtherBinding1;
            int TotalCost = InterestCost + UpkeepCost + AdvertisingCost + InventoryCost + (int)UpgradeCost + DebtCost;


            MarkupText.Text = "Markup: " + (game.Markup * 100) + "%"
                + Environment.NewLine + "Satisfaction Bonus: " + ((game.Markup - 0.5) * -5);
            UpgradeText.Text = "Upgrade Level: " + game.UpgradeLVL
                + Environment.NewLine + "Satisfaction Bonus: " + ((game.UpgradeLVL + 1.0) / 2.0)
                + Environment.NewLine + "Upgrade Cost: $" + UpgradeCost
                + Environment.NewLine + "Upkeep Cost: $" + UpkeepCost;
            AdvertisingText.Text = "Advert. Bonus Increase: " + ((game.AdvertBonus - gameOnFile.AdvertBonus) * 100)
                + Environment.NewLine + "Cost: $" + AdvertisingCost;
            InventoryText.Text = "Inventory: " + game.Inventory
                + Environment.NewLine + "Cost: $" + InventoryCost;

            string TowardsDebtText;
            if (game.OtherBinding1 > 0)
            {
                DebtText.Text = "Current Debt: $" + (game.CurrentDebt + game.OtherBinding1)
                    + Environment.NewLine + "Debt Change: +$" + game.OtherBinding1;
                TowardsDebtText = "Towards Debt: -$" + game.OtherBinding1;
            }
            else if (game.OtherBinding1 < 0)
            {
                DebtText.Text = "Current Debt: $" + (game.CurrentDebt + game.OtherBinding1)
                    + Environment.NewLine + "Debt Change: -$" + -game.OtherBinding1;
                TowardsDebtText = "Towards Debt: $" + -game.OtherBinding1;
            }
            else
            {
                DebtText.Text = "Current Debt: $" + (game.CurrentDebt + game.OtherBinding1)
                    + Environment.NewLine + "Debt Change: None";
                TowardsDebtText = "Towards Debt: $0";
            }


            ExpensesText.Text = $"Interest({game.Interest * 100}%): $" + InterestCost
                + Environment.NewLine + "Upkeep: $" + UpkeepCost + "$"
                + Environment.NewLine + "Advertising: $" + AdvertisingCost
                + Environment.NewLine + "Inventory: $" + InventoryCost
                + Environment.NewLine + "Upgrades: $" + UpgradeCost
                + Environment.NewLine + "" + TowardsDebtText
                + Environment.NewLine + "Total Expenses: $" + TotalCost
                + Environment.NewLine + "Current Cash: $" + gameOnFile.CurrentCash
                + Environment.NewLine + "New Total Cash: $" + (gameOnFile.CurrentCash - TotalCost);
            ErrorText.Text = "";

            if (game.CurrentTurn == 0)
            {
                MarkupDescriptionText.Text = "Markup is how much extra you charge for each book you sell. " +
                    "So, for example, if you sell a $10 book with 50% markup, then the customer will pay you $15, you will spend $10 on buying a new book, and you will have $5 of profit. " +
                    "However, the more you charge, the less people will want to buy your books, so they will not be as satisfied with your shop. " +
                    "(If your customers are not satisfied with your store then they will not buy books.) " +
                    "It does not cost anything to change.";
                UpgradeDescriptionText.Text = "Every upgrade increases your customer's satisfaction by 0.5. " +
                    "Unfortunately for you, each upgrade is also more expensive than the one before it. " +
                    "Each upgrade increases your store's upkeep cost as well. (this includes maintenance, utilities, rent, etc.) ";
                AdvertisingDescriptionText.Text = "Spending money on advertising increases your advertising bonus, which increases how many people will come to your store each turn. " +
                    "The more you increase your advertising though, the more it costs.";
                InventoryDescriptionText.Text = "Your inventory is how many of each book you have. " +
                    "If you don't have a book then you can't sell it, so this is important " +
                    "(if you don't have any inventory then you can't sell anything!) " +
                    "Inventory costs $200 to increase, and whenever you sell a book you automaticaly get another, thereby keeping the same amount of inventory";
                DebtDescriptionText.Text = "This is your debt. The first month (this month), you cannot change it. " +
                    "On a normal turn though, you can pay it off or you can increase it. " +
                    "(You can only take out as much debt as the amount of cash you currently have.) " +
                    "Your interest is paid on your debt, so paying it off has direct benifits.";
            }
            else
            {
                MarkupDescriptionText.Text = "(Markup does not cost anything to change)";
                UpgradeDescriptionText.Text = "(The cost of upgrading your shop and upkeep increases per upgrade)";
                AdvertisingDescriptionText.Text = "(Advertisement cost scales up as the advertising bonus increases)";
                InventoryDescriptionText.Text = "(It costs $200 to increase your inventory)";
                DebtDescriptionText.Text = "(You can only take out as much debt as the amount of cash you currently have)";
            }
        }
    }
}