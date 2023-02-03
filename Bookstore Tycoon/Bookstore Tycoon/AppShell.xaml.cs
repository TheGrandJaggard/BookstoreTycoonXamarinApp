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
            Routing.RegisterRoute(nameof(InterestChangePage), typeof(InterestChangePage));
            Routing.RegisterRoute(nameof(MonthlyManagementPage), typeof(MonthlyManagementPage));
            Routing.RegisterRoute(nameof(GameStatsPage), typeof(GameStatsPage));
            Routing.RegisterRoute(nameof(WeeklyTurnPage), typeof(WeeklyTurnPage));
            // Routing.RegisterRoute(nameof(Name), typeof(Name));
        }
    }
}