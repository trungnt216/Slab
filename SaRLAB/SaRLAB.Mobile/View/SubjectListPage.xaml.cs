using SaRLAB.Mobile.Models;
using SaRLAB.Mobile.ViewModels;

namespace SaRLAB.Mobile.View;

public partial class SubjectListPage : ContentPage
{
    public SubjectListPage()
    {
        InitializeComponent();
        BindingContext = new MenuViewModel();
    }

    private async void OnSubjectTapped(object sender, ItemTappedEventArgs e)
    {
        if (e.Item == null) return;

        var selectedSubject = e.Item as MenuItemModel;
        if (selectedSubject != null && selectedSubject.SubMenuItems.Count > 0)
        {
            var submenuPage = new SubjectMenuPage(selectedSubject);
            await Navigation.PushAsync(submenuPage);
        }

        ((ListView)sender).SelectedItem = null;
    }
}


