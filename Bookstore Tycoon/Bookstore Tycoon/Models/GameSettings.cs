using System;

namespace Bookstore_Tycoon.Models
{
    public class GameSettings
    {
        // this data is in the metadata of the file
        public string Filename { get; set; } // files are saved as "*.gamesettings.txt" in the App.FolderPath
        public DateTime Date { get; set; }
        // this data is in the file
        public string GameName { get; set; }        // line 0
        public bool RealDice { get; set; }          // line 1
        public int GameLength { get; set; }         // line 2
        public int StartingCash { get; set; }       // line 3
        public double MoneyMultiplier { get; set; } // line 4
        public bool RandomEvents { get; set; }      // line 5
        public double AdvertBase { get; set; }      // line 6
    }
}