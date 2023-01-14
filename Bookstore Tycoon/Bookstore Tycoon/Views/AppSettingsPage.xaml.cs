using System;
using System.IO;
using System.Diagnostics;
using Xamarin.Forms;

namespace Bookstore_Tycoon.Views
{
    public partial class AppSettingsPage : ContentPage
    {
        private string appTheme;
        private string filename = Path.Combine(App.FolderPath, "appsettings.txt");

        public AppSettingsPage()
        {
            InitializeComponent();
            ReciveDataFromFile();
        }

        void ReciveDataFromFile()
        {
            appTheme = File.ReadAllText(filename);
            if(string.IsNullOrWhiteSpace(appTheme))
            {
                appTheme = "Default";
                SendDataToFile();
            }

            if (appTheme == "Default") { AppDefaultThemeButton.IsChecked = true; }
            else if (appTheme == "Light") { AppLightThemeButton.IsChecked = true; }
            else if (appTheme == "Dark") { AppDarkThemeButton.IsChecked = true; }

            ChangeAppTheme();
        }

        void SendDataToFile()
        {
            File.WriteAllText(filename, appTheme);
        }

        void OnAppThemeDefault(object sender, EventArgs e)
        {
            appTheme = "Default";
            SendDataToFile();
            ChangeAppTheme();
        }

        void OnAppThemeLight(object sender, EventArgs e)
        {

            appTheme = "Light";
            SendDataToFile();
            ChangeAppTheme();
        }

        void OnAppThemeDark(object sender, EventArgs e)
        {

            appTheme = "Dark";
            SendDataToFile();
            ChangeAppTheme();
        }

        void ChangeAppTheme()
        {
            if(appTheme == "Default")
            {
                Application.Current.UserAppTheme = OSAppTheme.Unspecified;
            }
            else if (appTheme == "Light")
            {
                Application.Current.UserAppTheme = OSAppTheme.Light;
            }
            else if (appTheme == "Dark")
            {
                Application.Current.UserAppTheme = OSAppTheme.Dark;
            }
            else
            {
                Debug.WriteLine("Invalid app theme loading");
            }
        }
    }
}