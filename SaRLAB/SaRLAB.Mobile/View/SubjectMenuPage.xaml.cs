using SaRLAB.Mobile.Models;
using SaRLAB.Mobile.ViewModels;

namespace SaRLAB.Mobile.View
{
    public partial class SubjectMenuPage : ContentPage
    {
        public SubjectMenuPage(MenuItemModel menuItem)
        {
            InitializeComponent();
            BindingContext = new SubMenuViewModel(menuItem);
        }

        private async void OnSubMenuTapped(object sender, ItemTappedEventArgs e)
        {
            if (e.Item == null) return;

            var selectedSubMenuItem = e.Item as MenuItemModel;
            if (selectedSubMenuItem != null)
            {
                if (selectedSubMenuItem.SubMenuItems.Count > 0)
                {
                    var submenuPage = new SubjectMenuPage(selectedSubMenuItem);
                    await Navigation.PushAsync(submenuPage);
                }
                else if (selectedSubMenuItem.TargetType != null)
                {
                    var page = (Page)Activator.CreateInstance(selectedSubMenuItem.TargetType);
                    await Navigation.PushAsync(page);
                }
            }

            ((ListView)sender).SelectedItem = null;
        }
    }
}
