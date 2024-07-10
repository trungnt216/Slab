using SaRLAB.Mobile.Models;

namespace SaRLAB.Mobile.ViewModels
{
    public class SubMenuViewModel
    {
        public MenuItemModel MenuItem { get; set; }

        public SubMenuViewModel(MenuItemModel menuItem)
        {
            MenuItem = menuItem;
        }
    }
}
