using Bookstore_Tycoon.Views;
using Xamarin.Forms;

namespace Bookstore_Tycoon
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute(nameof(ChooseGamePage), typeof(ChooseGamePage));
            Routing.RegisterRoute(nameof(GameSettingsPage), typeof(GameSettingsPage));
            Routing.RegisterRoute(nameof(GameplayHomePage), typeof(GameplayHomePage));
        }
    }
}