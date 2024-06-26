using SaRLAB.Mobile.View;

namespace SaRLAB.Mobile
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            // Register the route for the SignInPage
            Routing.RegisterRoute("SignIn", typeof(SignInPage));

            // Navigate to the SignInPage on startup
            GoToAsync("SignIn");
        }
    }
}
