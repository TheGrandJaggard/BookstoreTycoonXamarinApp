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

            var games = new List<GameData>();

            // Create a Note object from each file.

            var files = Directory.EnumerateFiles(App.FolderPath, "*.gamedata.txt");

            foreach (string filename in files)
            {
                List<string> fileData = File.ReadAllLines(filename).ToList();

                #region FixingFiles (Disabled)
                /*
                int count = fileData.Count;

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
                */
                #endregion

                games.Add(new GameData
                {
                    // this is the only file data we need
                    Filename = filename,
                    Date = File.GetCreationTime(filename),
                    GameName = fileData[0]
                });
            }

            // Set the data source for the CollectionView to a sorted collection of games.
            collectionView.ItemsSource = games.OrderBy(d => d.Date).Reverse().ToList();
        }

        async void OnNewGameClicked(object sender, EventArgs e)
        {
            // Create a new file, fill it with defalult values, then go to GameSettingsPage
            string filename = Path.Combine(App.FolderPath, $"{Path.GetRandomFileName()}.gamedata.txt");

            #region Create new File (Disabled)
            /*
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
            */
            #endregion

            await Shell.Current.GoToAsync($"{nameof(GameSettingsPage)}?{nameof(GameSettingsPage.GameID)}={filename}");
        }

        async void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.CurrentSelection != null)
            {
                // find the game from the selection then go to GameSettingsPage
                GameData game = (GameData)e.CurrentSelection.FirstOrDefault();
                await Shell.Current.GoToAsync($"{nameof(GameplayHomePage)}?{nameof(GameplayHomePage.GameID)}={game.Filename}");
            }
        }
    }
}