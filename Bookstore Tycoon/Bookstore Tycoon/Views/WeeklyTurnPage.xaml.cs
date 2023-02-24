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
    public partial class WeeklyTurnPage : ContentPage
    {
        private static readonly Random rnd = new Random();

        public string GameID
        {
            set
            {
                LoadGame(value);
                UpdateBindings();
            }
        }

        public WeeklyTurnPage()
        {
            InitializeComponent();
            // Set the BindingContext of the page to a new set of game stats.
            BindingContext = new RollingInfo();
        }

        void LoadGame(string filename)
        {
            try
            {
                if (!filename.EndsWith(".gamedata.txt"))
                {
                    throw new Exception();
                }

                RollingInfo rolls = new RollingInfo
                {
                    Filename = filename,
                    GameName = File.ReadAllLines(filename)[0],
                };
                BindingContext = rolls;

                SatisfactionRollButton.IsEnabled = false;
                BookRollButton.IsEnabled = false;
            }
            catch (Exception)
            {
                Debug.WriteLine("Failed to load game!");
            }
        }

        async void OnContinueButtonClicked(object sender, EventArgs e)
        {
            RollingInfo rolls = (RollingInfo)BindingContext;

            List<string> fileData = File.ReadAllLines(rolls.Filename).ToList();
            GameData game = new GameData
            {
                Filename = rolls.Filename,
                Date = File.GetCreationTime(rolls.Filename),
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
                SatisfactionBonus = (Convert.ToInt32(fileData[12]) + 1) / 2 + ((Convert.ToDouble(fileData[8]) - 0.5) * -5)
            };

            // insert maths here

            double AdvertBonusIncrease = 0;
            int BooksSold = 0;

            #region Dealing with Satisfaction
            if (game.SatisfactionBonus + 0 >= 4) { BooksSold += rolls.SatisfactionRoll0; }
            if (game.SatisfactionBonus + 1 >= 4) { BooksSold += rolls.SatisfactionRoll1; }
            if (game.SatisfactionBonus + 1 >= 4) { BooksSold += rolls.SatisfactionRoll2; }
            if (game.SatisfactionBonus + 2 >= 4) { BooksSold += rolls.SatisfactionRoll3; }
            if (game.SatisfactionBonus + 2 >= 4) { BooksSold += rolls.SatisfactionRoll4; }
            if (game.SatisfactionBonus + 3 >= 4) { BooksSold += rolls.SatisfactionRoll5; }

            if (game.SatisfactionBonus + 0 >= 6) { AdvertBonusIncrease += rolls.SatisfactionRoll0 * 0.1; }
            if (game.SatisfactionBonus + 1 >= 6) { AdvertBonusIncrease += rolls.SatisfactionRoll1 * 0.1; }
            if (game.SatisfactionBonus + 1 >= 6) { AdvertBonusIncrease += rolls.SatisfactionRoll2 * 0.1; }
            if (game.SatisfactionBonus + 2 >= 6) { AdvertBonusIncrease += rolls.SatisfactionRoll3 * 0.1; }
            if (game.SatisfactionBonus + 2 >= 6) { AdvertBonusIncrease += rolls.SatisfactionRoll4 * 0.1; }
            if (game.SatisfactionBonus + 3 >= 6) { AdvertBonusIncrease += rolls.SatisfactionRoll5 * 0.1; }

            if (game.SatisfactionBonus + 0 >= 8) { BooksSold += rolls.SatisfactionRoll0; }
            if (game.SatisfactionBonus + 1 >= 8) { BooksSold += rolls.SatisfactionRoll1; }
            if (game.SatisfactionBonus + 1 >= 8) { BooksSold += rolls.SatisfactionRoll2; }
            if (game.SatisfactionBonus + 2 >= 8) { BooksSold += rolls.SatisfactionRoll3; }
            if (game.SatisfactionBonus + 2 >= 8) { BooksSold += rolls.SatisfactionRoll4; }
            if (game.SatisfactionBonus + 3 >= 8) { BooksSold += rolls.SatisfactionRoll5; }
            #endregion

            #region Dealing with Inventory limits
            int BookRoll0Gained;
            if (rolls.BookRoll0 > game.Inventory)
            { BookRoll0Gained = game.Inventory; }
            else { BookRoll0Gained = rolls.BookRoll0; }
            int BookRoll1Gained;
            if (rolls.BookRoll1 > game.Inventory)
            { BookRoll1Gained = game.Inventory; }
            else { BookRoll1Gained = rolls.BookRoll1; }
            int BookRoll2Gained;
            if (rolls.BookRoll2 > game.Inventory)
            { BookRoll2Gained = game.Inventory; }
            else { BookRoll2Gained = rolls.BookRoll2; }
            int BookRoll3Gained;
            if (rolls.BookRoll3 > game.Inventory)
            { BookRoll3Gained = game.Inventory; }
            else { BookRoll3Gained = rolls.BookRoll3; }
            int BookRoll4Gained;
            if (rolls.BookRoll4 > game.Inventory)
            { BookRoll4Gained = game.Inventory; }
            else { BookRoll4Gained = rolls.BookRoll4; }
            int BookRoll5Gained;
            if (rolls.BookRoll5 > game.Inventory)
            { BookRoll5Gained = game.Inventory; }
            else { BookRoll5Gained = rolls.BookRoll5; }
            #endregion Dealing with Inventory limits

            int CashIncrease = (int)(((BookRoll0Gained * 20) + (BookRoll1Gained * 30) + (BookRoll2Gained * 40) +
                                       (BookRoll3Gained * 50) + (BookRoll4Gained * 60) + (BookRoll5Gained * 70))
                               * game.Markup * game.MoneyMultiplier);

            // We delete the file to clear it then make a new one with the same name
            if (File.Exists(game.Filename))
            {
                File.Delete(game.Filename);
            }

            // Here's the data we are going to write to the new file
            var newFileData = new List<string>
            {
                game.GameName,
                //game.RealDice.ToString(),
                game.GameLength.ToString(),
                game.StartingCash.ToString(),
                game.MoneyMultiplier.ToString(),
                game.RandomEvents.ToString(),
                game.AdvertBase.ToString(),
                (game.CurrentCash + CashIncrease).ToString(),
                game.CurrentDebt.ToString(),
                game.Markup.ToString(),
                (game.AdvertBonus + AdvertBonusIncrease).ToString(),
                game.Interest.ToString(),
                game.Inventory.ToString(),
                game.UpgradeLVL.ToString(),
                (game.CurrentTurn + 1).ToString()
            };

            // Put our data into the file
            File.AppendAllLines(rolls.Filename, newFileData);

            // Navigate backwards
            await Shell.Current.GoToAsync($"{nameof(GameplayHomePage)}?{nameof(GameplayHomePage.GameID)}={rolls.Filename}");
        }

        async void OnGoToStatsPageClicked(object sender, EventArgs e)
        {
            var rolls = (RollingInfo)BindingContext;
            await Shell.Current.GoToAsync($"{nameof(GameStatsPage)}?{nameof(GameStatsPage.GameID)}={rolls.Filename}");
        }

        void OnAdvertisingRollClicked(object sender, EventArgs e)
        {
            RollingInfo rolls = (RollingInfo)BindingContext;
            List<string> fileData = File.ReadAllLines(rolls.Filename).ToList();
            GameData game = new GameData
            {
                Filename = rolls.Filename,
                Date = File.GetCreationTime(rolls.Filename),
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
                SatisfactionBonus = (Convert.ToInt32(fileData[12]) + 1) / 2 + ((Convert.ToDouble(fileData[8]) - 0.5) * -5)
            };

            RefreshSatisfactionValues();

            rolls.Customers = 0;
            for (int i = 0; i < game.AdvertTotal; i++)
            {
                int temp = rnd.Next(0, 6);
                if (temp == 5)
                {
                    rolls.Customers += 2;
                }
                else if (temp != 0)
                {
                    rolls.Customers += 1;
                }
            }

            AdvertisingRollButton.IsEnabled = false;
            CustomersStepper.IsEnabled = false;
            SatisfactionRollButton.IsEnabled = true;

            UpdateBindings();
        }

        void OnSatisfactionRollClicked(object sender, EventArgs e)
        {
            RollingInfo rolls = (RollingInfo)BindingContext;

            RefreshSatisfactionValues();

            for (int i = 0; i < rolls.Customers; i++)
            {
                int temp = rnd.Next(0, 6);
                switch (temp)
                {
                    case 0:
                        rolls.SatisfactionRoll0 += 1;
                        break;
                    case 1:
                        rolls.SatisfactionRoll1 += 1;
                        break;
                    case 2:
                        rolls.SatisfactionRoll2 += 1;
                        break;
                    case 3:
                        rolls.SatisfactionRoll3 += 1;
                        break;
                    case 4:
                        rolls.SatisfactionRoll4 += 1;
                        break;
                    case 5:
                        rolls.SatisfactionRoll5 += 1;
                        break;
                }
            }

            SatisfactionRollButton.IsEnabled = false;
            SatisfactionRoll0Stepper.IsEnabled = false;
            SatisfactionRoll1Stepper.IsEnabled = false;
            SatisfactionRoll2Stepper.IsEnabled = false;
            SatisfactionRoll3Stepper.IsEnabled = false;
            SatisfactionRoll4Stepper.IsEnabled = false;
            SatisfactionRoll5Stepper.IsEnabled = false;
            BookRollButton.IsEnabled = true;

            UpdateBindings();
        }

        void OnBookRollClicked(object sender, EventArgs e)
        {
            RollingInfo rolls = (RollingInfo)BindingContext;

            RefreshBookValues();

            for (int i = 0; i < rolls.BooksSold; i++)
            {
                int temp = rnd.Next(0, 6);
                switch (temp)
                {
                    case 0:
                        rolls.BookRoll0 += 1;
                        break;
                    case 1:
                        rolls.BookRoll1 += 1;
                        break;
                    case 2:
                        rolls.BookRoll2 += 1;
                        break;
                    case 3:
                        rolls.BookRoll3 += 1;
                        break;
                    case 4:
                        rolls.BookRoll4 += 1;
                        break;
                    case 5:
                        rolls.BookRoll5 += 1;
                        break;
                }
            }

            BookRollButton.IsEnabled = false;
            BookRoll0Stepper.IsEnabled = false;
            BookRoll1Stepper.IsEnabled = false;
            BookRoll2Stepper.IsEnabled = false;
            BookRoll3Stepper.IsEnabled = false;
            BookRoll4Stepper.IsEnabled = false;
            BookRoll5Stepper.IsEnabled = false;

            UpdateBindings();
        }

        void OnUpdateBindings(object sender, ValueChangedEventArgs e)
        {
            UpdateBindings();
        }
        void OnRefreshSatisfactionValues(object sender, ValueChangedEventArgs e)
        {
            RefreshSatisfactionValues();
        }
        void OnRefreshBookValues(object sender, ValueChangedEventArgs e)
        {
            RefreshBookValues();
        }

        void UpdateBindings()
        {
            RollingInfo rolls = (RollingInfo)BindingContext;

            List<string> fileData = File.ReadAllLines(rolls.Filename).ToList();
            GameData game = new GameData
            {
                Filename = rolls.Filename,
                Date = File.GetCreationTime(rolls.Filename),
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
                SatisfactionBonus = (Convert.ToInt32(fileData[12]) + 1) / 2 + ((Convert.ToDouble(fileData[8]) - 0.5) * -5)
            };

            double AdvertBonusIncrease = 0;
            rolls.BooksSold = 0;

            #region Dealing with Satisfaction
            if (game.SatisfactionBonus + 0 >= 4) { rolls.BooksSold += rolls.SatisfactionRoll0; }
            if (game.SatisfactionBonus + 1 >= 4) { rolls.BooksSold += rolls.SatisfactionRoll1; }
            if (game.SatisfactionBonus + 1 >= 4) { rolls.BooksSold += rolls.SatisfactionRoll2; }
            if (game.SatisfactionBonus + 2 >= 4) { rolls.BooksSold += rolls.SatisfactionRoll3; }
            if (game.SatisfactionBonus + 2 >= 4) { rolls.BooksSold += rolls.SatisfactionRoll4; }
            if (game.SatisfactionBonus + 3 >= 4) { rolls.BooksSold += rolls.SatisfactionRoll5; }

            if (game.SatisfactionBonus + 0 >= 6) { AdvertBonusIncrease += rolls.SatisfactionRoll0 * 0.1; }
            if (game.SatisfactionBonus + 1 >= 6) { AdvertBonusIncrease += rolls.SatisfactionRoll1 * 0.1; }
            if (game.SatisfactionBonus + 1 >= 6) { AdvertBonusIncrease += rolls.SatisfactionRoll2 * 0.1; }
            if (game.SatisfactionBonus + 2 >= 6) { AdvertBonusIncrease += rolls.SatisfactionRoll3 * 0.1; }
            if (game.SatisfactionBonus + 2 >= 6) { AdvertBonusIncrease += rolls.SatisfactionRoll4 * 0.1; }
            if (game.SatisfactionBonus + 3 >= 6) { AdvertBonusIncrease += rolls.SatisfactionRoll5 * 0.1; }

            if (game.SatisfactionBonus + 0 >= 8) { rolls.BooksSold += rolls.SatisfactionRoll0; }
            if (game.SatisfactionBonus + 1 >= 8) { rolls.BooksSold += rolls.SatisfactionRoll1; }
            if (game.SatisfactionBonus + 1 >= 8) { rolls.BooksSold += rolls.SatisfactionRoll2; }
            if (game.SatisfactionBonus + 2 >= 8) { rolls.BooksSold += rolls.SatisfactionRoll3; }
            if (game.SatisfactionBonus + 2 >= 8) { rolls.BooksSold += rolls.SatisfactionRoll4; }
            if (game.SatisfactionBonus + 3 >= 8) { rolls.BooksSold += rolls.SatisfactionRoll5; }
            #endregion

            #region Dealing with Inventory limits
            int BookRoll0Gained;
            if (rolls.BookRoll0 > game.Inventory)
            { BookRoll0Gained = game.Inventory; }
            else { BookRoll0Gained = rolls.BookRoll0; }
            int BookRoll1Gained;
            if (rolls.BookRoll1 > game.Inventory)
            { BookRoll1Gained = game.Inventory; }
            else { BookRoll1Gained = rolls.BookRoll1; }
            int BookRoll2Gained;
            if (rolls.BookRoll2 > game.Inventory)
            { BookRoll2Gained = game.Inventory; }
            else { BookRoll2Gained = rolls.BookRoll2; }
            int BookRoll3Gained;
            if (rolls.BookRoll3 > game.Inventory)
            { BookRoll3Gained = game.Inventory; }
            else { BookRoll3Gained = rolls.BookRoll3; }
            int BookRoll4Gained;
            if (rolls.BookRoll4 > game.Inventory)
            { BookRoll4Gained = game.Inventory; }
            else { BookRoll4Gained = rolls.BookRoll4; }
            int BookRoll5Gained;
            if (rolls.BookRoll5 > game.Inventory)
            { BookRoll5Gained = game.Inventory; }
            else { BookRoll5Gained = rolls.BookRoll5; }
            #endregion Dealing with Inventory limits

            #region Limiting maximums and setting values for Steppers

            CustomersStepper.Maximum = game.AdvertTotal * 2;


            int SatisfactionRollsLeft = rolls.Customers - (rolls.SatisfactionRoll0 + rolls.SatisfactionRoll1 + rolls.SatisfactionRoll2
                                                        + rolls.SatisfactionRoll3 + rolls.SatisfactionRoll4 + rolls.SatisfactionRoll5);

            SatisfactionRoll0Stepper.Value = rolls.SatisfactionRoll0;
            SatisfactionRoll1Stepper.Value = rolls.SatisfactionRoll1;
            SatisfactionRoll2Stepper.Value = rolls.SatisfactionRoll2;
            SatisfactionRoll3Stepper.Value = rolls.SatisfactionRoll3;
            SatisfactionRoll4Stepper.Value = rolls.SatisfactionRoll4;
            SatisfactionRoll5Stepper.Value = rolls.SatisfactionRoll5;

            SatisfactionRoll0Stepper.Maximum = rolls.SatisfactionRoll0 + SatisfactionRollsLeft;
            SatisfactionRoll1Stepper.Maximum = rolls.SatisfactionRoll1 + SatisfactionRollsLeft;
            SatisfactionRoll2Stepper.Maximum = rolls.SatisfactionRoll2 + SatisfactionRollsLeft;
            SatisfactionRoll3Stepper.Maximum = rolls.SatisfactionRoll3 + SatisfactionRollsLeft;
            SatisfactionRoll4Stepper.Maximum = rolls.SatisfactionRoll4 + SatisfactionRollsLeft;
            SatisfactionRoll5Stepper.Maximum = rolls.SatisfactionRoll5 + SatisfactionRollsLeft;

            int BookRollsLeft = rolls.BooksSold - (rolls.BookRoll0 + rolls.BookRoll1 + rolls.BookRoll2
                                          + rolls.BookRoll3 + rolls.BookRoll4 + rolls.BookRoll5);

            BookRoll0Stepper.Value = rolls.BookRoll0;
            BookRoll1Stepper.Value = rolls.BookRoll1;
            BookRoll2Stepper.Value = rolls.BookRoll2;
            BookRoll3Stepper.Value = rolls.BookRoll3;
            BookRoll4Stepper.Value = rolls.BookRoll4;
            BookRoll5Stepper.Value = rolls.BookRoll5;

            BookRoll0Stepper.Maximum = rolls.BookRoll0 + BookRollsLeft;
            BookRoll1Stepper.Maximum = rolls.BookRoll1 + BookRollsLeft;
            BookRoll2Stepper.Maximum = rolls.BookRoll2 + BookRollsLeft;
            BookRoll3Stepper.Maximum = rolls.BookRoll3 + BookRollsLeft;
            BookRoll4Stepper.Maximum = rolls.BookRoll4 + BookRollsLeft;
            BookRoll5Stepper.Maximum = rolls.BookRoll5 + BookRollsLeft;
            #endregion

            int CashIncrease = (int)( ( (BookRoll0Gained * 20) + (BookRoll1Gained * 30) + (BookRoll2Gained * 40) +
                                       (BookRoll3Gained * 50) + (BookRoll4Gained * 60) + (BookRoll5Gained * 70) )
                                       * game.Markup * game.MoneyMultiplier);
            #region Books Lost
            int TotalBooksGained = BookRoll0Gained + BookRoll1Gained + BookRoll2Gained + BookRoll3Gained + BookRoll4Gained + BookRoll5Gained;
            int BooksLost = rolls.BookRoll0 + rolls.BookRoll1 + rolls.BookRoll2 + rolls.BookRoll3 + rolls.BookRoll4 + rolls.BookRoll5
                            - TotalBooksGained;

            string BooksLostText = "This round you did not lose any books!";
            if (BooksLost != 0)
            {
                BooksLostText = BooksLost == 1 ? $"But... you also lost {BooksLost} book." : $"But... you also lost {BooksLost} books.";
            }
            #endregion

            if (game.CurrentTurn == 1)
            {
                AdvertisingRoll.Text = $"Every turn you start by rolling your 'advertising' die. " +
                    $"The number of die you roll is dependant on your advertisment total, which for you is {game.AdvertTotal}. " +
                    $"Once you have rolled the all die (roll them all at once), remove each dice that shows 1 and add an extra dice for each 6. " +
                    $"(The 1s don't come to the store and the 6s bring friends.) " +
                    $"Now, each dice represents one customer in your store. " +
                    $"Enter that number below and start rolling for satisfaction";

                CustomersText.Text = $"Customers:    {rolls.Customers}";

                SatisfactionRoll.Text = $"Roll your handful of dice, each representing one customer in your store. " +
                    $"(For you that is {rolls.Customers} dice.) " +
                    $"Enter your rolls below.";

                SatisfactionRoll0Text.Text = $"Rolls of 1 (Satisfaction +0):    {rolls.SatisfactionRoll0}";
                SatisfactionRoll1Text.Text = $"Rolls of 2 (Satisfaction +1):    {rolls.SatisfactionRoll1}";
                SatisfactionRoll2Text.Text = $"Rolls of 3 (Satisfaction +1):    {rolls.SatisfactionRoll2}";
                SatisfactionRoll3Text.Text = $"Rolls of 4 (Satisfaction +2):    {rolls.SatisfactionRoll3}";
                SatisfactionRoll4Text.Text = $"Rolls of 5 (Satisfaction +2):    {rolls.SatisfactionRoll4}";
                SatisfactionRoll5Text.Text = $"Rolls of 6 (Satisfaction +3):    {rolls.SatisfactionRoll5}";

                SatsifactionResults.Text = $"The roll that you just made is the 'Satisfaction Roll.' " +
                    $"This effects how happy people are with your store. " +
                    $"Each customer's satisfaction roll result is added to your satisfaction bonus. " +
                    $"(For you this is {game.SatisfactionBonus}.) " +
                    $"If the total satisfaction of a customer is four or greater, then they buy a book. " +
                    $"If it is six or greater they they also leave a good review and your advertisment bonus goes up 1%. " +
                    $"If it is eight or greater than they will buy a second book on top of all that! " +
                    $"So for you this week it means that you sell {rolls.BooksSold} books and your advertisment bonus goes up {AdvertBonusIncrease}% from good reviews. " +
                    $"(It is normal for this to be zero at the beginning of the game.)";

                BookRoll.Text = $"Now roll a dice for each book you are selling this week. " +
                    $"(That would be {rolls.BooksSold} dice.) " +
                    $"This is to determine what price of books your customers want. " +
                    $"As always, enter those results below so we can do all the fun calculatons for you. ";
                BookRoll0Text.Text = $"Rolls of 1:    {rolls.BookRoll0}" +
                    Environment.NewLine + $"These are magazines. They have wholesale values of $20";
                BookRoll1Text.Text = $"Rolls of 2:    {rolls.BookRoll1}" +
                    Environment.NewLine + $"These are pulp fictions. They have wholesale values of $30";
                BookRoll2Text.Text = $"Rolls of 3:    {rolls.BookRoll2}" +
                    Environment.NewLine + $"These are paperbacks. They have wholesale values of $40";
                BookRoll3Text.Text = $"Rolls of 4:    {rolls.BookRoll3}" +
                    Environment.NewLine + $"These are hardcovers. They have wholesale values of $50";
                BookRoll4Text.Text = $"Rolls of 5:    {rolls.BookRoll4}" +
                    Environment.NewLine + $"These are leatherbound books. They have wholesale values of $60";
                BookRoll5Text.Text = $"Rolls of 6:    {rolls.BookRoll5}" +
                    Environment.NewLine + $"These are boxed sets. They have wholesale values of $70";

                BooksResults.Text = $"Each book you sell does not profit you the wholesale value though, that is how much you buy the book from the publisher for. " +
                    $"Instead, your profit is your markup percentage of that. " +
                    $"So for example, you buy a paperback from the publisher for $40. " +
                    $"Then, because your markup is {game.Markup * 100}% you sell it to a customer for ${40 + (40 * game.Markup)}. " +
                    $"You then buy another book from the publiser for $40, leaving yourself ${40 * game.Markup} of profit. " +
                    Environment.NewLine + $"In total this turn, you sold {TotalBooksGained} books, and gained ${CashIncrease}. " +
                    $"Because of the inventory limit, if you try to sell more books in any one catagory than your inventory limit, you lose those sales. " +
                    $"Because of this, you lost {BooksLost} sale(s).";
            }
            else
            {
                AdvertisingRoll.Text = $"Roll {game.AdvertTotal} dice for advertising. " +
                    $"Enter the result below. This is how many people come into your store.";

                CustomersText.Text = $"Customers:    {rolls.Customers}";

                SatisfactionRoll.Text = $"Now roll {rolls.Customers} dice and enter your rolls below.";

                SatisfactionRoll0Text.Text = $"Rolls of 1:    {rolls.SatisfactionRoll0}";
                SatisfactionRoll1Text.Text = $"Rolls of 2:    {rolls.SatisfactionRoll1}";
                SatisfactionRoll2Text.Text = $"Rolls of 3:    {rolls.SatisfactionRoll2}";
                SatisfactionRoll3Text.Text = $"Rolls of 4:    {rolls.SatisfactionRoll3}";
                SatisfactionRoll4Text.Text = $"Rolls of 5:    {rolls.SatisfactionRoll4}";
                SatisfactionRoll5Text.Text = $"Rolls of 6:    {rolls.SatisfactionRoll5}";

                SatsifactionResults.Text = $"This week you sell {rolls.BooksSold} books." +
                    Environment.NewLine + $"Your advertisment bonus goes up {AdvertBonusIncrease}% from good reviews.";

                BookRoll.Text = $"Roll {rolls.BooksSold} dice. What number of each roll did you get?";
                BookRoll0Text.Text = $"Rolls of 1:    {rolls.BookRoll0}";
                BookRoll1Text.Text = $"Rolls of 2:    {rolls.BookRoll1}";
                BookRoll2Text.Text = $"Rolls of 3:    {rolls.BookRoll2}";
                BookRoll3Text.Text = $"Rolls of 4:    {rolls.BookRoll3}";
                BookRoll4Text.Text = $"Rolls of 5:    {rolls.BookRoll4}";
                BookRoll5Text.Text = $"Rolls of 6:    {rolls.BookRoll5}";

                BooksResults.Text = $"Results: You sold {TotalBooksGained} books, gaining ${CashIncrease}." +
                    Environment.NewLine + $"{BooksLostText}";
            }
        }

        void RefreshSatisfactionValues()
        {
            RollingInfo rolls = (RollingInfo)BindingContext;
            rolls.SatisfactionRoll0 = 0;
            rolls.SatisfactionRoll1 = 0;
            rolls.SatisfactionRoll2 = 0;
            rolls.SatisfactionRoll3 = 0;
            rolls.SatisfactionRoll4 = 0;
            rolls.SatisfactionRoll5 = 0;

            RefreshBookValues();
        }

        void RefreshBookValues()
        {
            RollingInfo rolls = (RollingInfo)BindingContext;
            rolls.BookRoll0 = 0;
            rolls.BookRoll1 = 0;
            rolls.BookRoll2 = 0;
            rolls.BookRoll3 = 0;
            rolls.BookRoll4 = 0;
            rolls.BookRoll5 = 0;

            UpdateBindings();
        }
    }
}