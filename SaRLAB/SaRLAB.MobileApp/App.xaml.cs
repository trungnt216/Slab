using SaRLAB.MobileApp.Models;

namespace SaRLAB.MobileApp
{
    public partial class App : Application
    {
        public static User user;

        public App()
        {
            InitializeComponent();

            MainPage = new AppShell();
        }
    }
}
