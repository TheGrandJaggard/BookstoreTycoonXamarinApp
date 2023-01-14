using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Bookstore_Tycoon.Models;
using Xamarin.Forms;

namespace Bookstore_Tycoon.Views
{
    public partial class ChooseGamePage : ContentPage
    {
        public ChooseGamePage()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            var games = new List<GameSettings>();

            // Create a Note object from each file.

            var files = Directory.EnumerateFiles(App.FolderPath, "*.gamesettings.txt");

            Debug.WriteLine($"This is the total number of files: {files.ToList().Count}");
            foreach (string filename in files)
            {
                List<string> fileData = File.ReadAllLines(filename).ToList();

                // ERRORS HERE
                int count = fileData.Count;

                #region FixingFiles
                if (count == 0)
                {
                    fileData.Add("Unnamed Game");
                    File.AppendAllLines(filename, new List<string> { fileData[count] });
                    Debug.WriteLine($"Just fixed line {count}, it now equals '{fileData[count]}' ");
                    count++;
                }
                if (count == 1)
                {
                    fileData.Add("true");
                    File.AppendAllLines(filename, new List<string> { fileData[count] });
                    Debug.WriteLine($"Just fixed line {count}, it now equals '{fileData[count]}' ");
                    count++;
                }
                if (count == 2)
                {
                    fileData.Add("6");
                    File.AppendAllLines(filename, new List<string> { fileData[count] });
                    Debug.WriteLine($"Just fixed line {count}, it now equals '{fileData[count]}' ");
                    count++;
                }
                if (count == 3)
                {
                    fileData.Add("500");
                    File.AppendAllLines(filename, new List<string> { fileData[count] });
                    Debug.WriteLine($"Just fixed line {count}, it now equals '{fileData[count]}' ");
                    count++;
                }
                if (count == 4)
                {
                    fileData.Add("1.00");
                    File.AppendAllLines(filename, new List<string> { fileData[count] });
                    Debug.WriteLine($"Just fixed line {count}, it now equals '{fileData[count]}' ");
                    count++;
                }
                if (count == 5)
                {
                    fileData.Add("true");
                    File.AppendAllLines(filename, new List<string> { fileData[count] });
                    Debug.WriteLine($"Just fixed line {count}, it now equals '{fileData[count]}' ");
                    count++;
                }
                if (count == 6)
                {
                    fileData.Add("5.00");
                    File.AppendAllLines(filename, new List<string> { fileData[count] });
                    Debug.WriteLine($"Just fixed line {count}, it now equals '{fileData[count]}' ");
                    count++;
                } 
                #endregion

                games.Add(new GameSettings
                {
                    // file metadata
                    Filename = filename,
                    Date = File.GetCreationTime(filename),
                    // file data
                    GameName = fileData[0],
                    RealDice = Convert.ToBoolean(fileData[1]),
                    GameLength = Convert.ToInt32(fileData[2]),
                    StartingCash = Convert.ToInt32(fileData[3]),
                    MoneyMultiplier = Convert.ToDouble(fileData[4]),
                    RandomEvents = Convert.ToBoolean(fileData[5]),
                    AdvertBase = Convert.ToDouble(fileData[6])
                });
            }

            // Set the data source for the CollectionView to a sorted collection of games.
            collectionView.ItemsSource = games
                .OrderBy(d => d.Date)
                .ToList();
        }

        void OnAddClicked(object sender, EventArgs e)
        {
            // Create a new file, fill it with defalult values, then Go To Game Settings Page
            string filename = Path.Combine(App.FolderPath, $"{Path.GetRandomFileName()}.gamesettings.txt");

            var fileData = new List<string>
            {
                "Unnamed Game",
                "true",
                "6",
                "500",
                "1.00",
                "true",
                "5.00"
            };
            File.WriteAllLines(filename, fileData);

            GoToGameSettingsPage(filename);
        }

        void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.CurrentSelection != null)
            {
                // find the game from the selection then Go To Game Settings Page
                GameSettings game = (GameSettings)e.CurrentSelection.FirstOrDefault();
                GoToGameSettingsPage(game.Filename);
            }
        }

        private async void GoToGameSettingsPage(string filename)
        {

            // Navigate to the GameSettingsPage, passing the filename as a query parameter.
            await Shell.Current.GoToAsync($"{nameof(GameSettingsPage)}?{nameof(GameSettingsPage.ItemId)}={filename}");
        }
    }
}