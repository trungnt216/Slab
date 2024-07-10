namespace SaRLAB.Mobile.Models;

public class MenuItemModel
{
    public string Title { get; set; }
    public Type TargetType { get; set; }
    public List<MenuItemModel> SubMenuItems { get; set; }

    public MenuItemModel()
    {
        SubMenuItems = new List<MenuItemModel>();
    }
}
