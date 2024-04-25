using SaRLAB.MobileApp.ViewModels;
using System.ComponentModel;

namespace SaRLAB.MobileApp;

public partial class LoginPage : ContentPage
{
	public LoginPage(LoginPageViewModel vm)
	{
		InitializeComponent();
		BindingContext = vm;
	}
}