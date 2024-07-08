using SaRLAB.Mobile.View;

namespace SaRLAB.Mobile
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new NavigationPage(new SignInPage());
            NavigationPage.SetHasNavigationBar(this, false);
        }
    }
}
