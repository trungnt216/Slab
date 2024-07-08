namespace SaRLAB.Mobile.View;

public partial class SignUpPage : ContentPage
{
	public SignUpPage()
	{
		InitializeComponent();
	}

    private async void OnSignInClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new SignInPage());
    }

    private void OnSchoolPickerSelectedIndexChanged(object sender, EventArgs e)
    {
    }

}