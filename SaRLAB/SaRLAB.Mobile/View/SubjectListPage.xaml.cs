using SaRLAB.Mobile.Models;
using SaRLAB.Mobile.View.Biochem;
using SaRLAB.Mobile.View.Biology;
using SaRLAB.Mobile.View.Chemistry;
using SaRLAB.Mobile.View.Physics;
using SaRLAB.Mobile.ViewModels;

namespace SaRLAB.Mobile.View;

public partial class SubjectListPage : ContentPage
{
    public SubjectListPage()
    {
        InitializeComponent();
        BindingContext = new SubjectViewModel();
        //BindingContext = new MenuViewModel();
    }

    private async void OnSubjectTapped(object sender, EventArgs e)
    {
        if (sender is Frame frame && frame.BindingContext is SubjectModel subject)
        {
            switch (subject.SubjectName)
            {
                case "CHemLAB":
                    await Navigation.PushAsync(new Chem_QuizPage());
                    break;
                case "BiOLAB":
                    await Navigation.PushAsync(new Bio_QuizPage());
                    break;
                case "PhysLAB":
                    await Navigation.PushAsync(new Phys_QuizPage());
                    break;
                case "BioChemLAB":
                    await Navigation.PushAsync(new BioChem_QuizPage());
                    break;
                // Add more cases for other subjects
                default:
                    await DisplayAlert("Info", "Page not implemented yet.", "OK");
                    break;
            }
        }
    }
}


