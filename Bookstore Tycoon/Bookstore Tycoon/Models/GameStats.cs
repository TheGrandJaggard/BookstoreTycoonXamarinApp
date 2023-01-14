using System;

namespace Bookstore_Tycoon.Models
{
    public class GameStats
    {
        // This data is in the metadata of the file
        public string Filename { get; set; } // files are saved as "*.gamesettings.txt" in the App.FolderPath
        public DateTime Date { get; set; }

        // This data is in the file
        //     This data comes straight from the GameSettings file and does not change
        public string GameName { get; set; }        // stored on line 0
        public bool RealDice { get; set; }          // stored on line 1
        public int GameLength { get; set; }         // stored on line 2
        public int StartingCash { get; set; }       // stored on line 3
        public double MoneyMultiplier { get; set; } // stored on line 4
        public bool RandomEvents { get; set; }      // stored on line 5
        public double AdvertBase { get; set; }      // stored on line 6
        //     This data changes thorought the game
        public int CurrentCash { get; set; }        // stored on line 7
        public int CurrentDebt { get; set; }        // stored on line 8
        public double Markup { get; set; }          // stored on line 9
        public double AdvertBonus { get; set; }     // stored on line 10
        public double Interest { get; set; }        // stored on line 12
        public int Inventory { get; set; }          // stored on line 13
        public int Score { get; set; }              // stored on line 14
        public int UpgradeLVL { get; set; }         // stored on line 15
        //     This data is not stored in the file, it is derived solely from the other stats
        public string UpgradeName { get; set; }     // derived from a list and UpgradeLVL
        public int AdvertTotal { get; set; }        // derived from advert base and AdvertBonus
        public int SatisfactionBonus { get; set; }  // derived from Markup and UpgradeLVL
    }
}