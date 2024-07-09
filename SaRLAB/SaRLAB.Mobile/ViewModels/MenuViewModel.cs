using SaRLAB.Mobile.Models;
using SaRLAB.Mobile.View.Biochem;
using SaRLAB.Mobile.View.Biology;
using SaRLAB.Mobile.View.Chemistry;
using SaRLAB.Mobile.View.Physics;
using System.Collections.ObjectModel;

public class MenuViewModel
{
    public ObservableCollection<MenuItemModel> MenuItems { get; set; }

    public MenuViewModel()
    {
        MenuItems = new ObservableCollection<MenuItemModel>
        {
            new MenuItemModel
            {
                Title = "Chemistry",
                SubMenuItems = new List<MenuItemModel>
                {
                    new MenuItemModel
                    {
                        Title = "Giáo trình thực hành",
                        SubMenuItems = new List<MenuItemModel>
                        {
                            new MenuItemModel { Title = "Lý thuyết", TargetType = typeof(Chem_GetAllTheory) },
                            new MenuItemModel { Title = "Thực hành", TargetType = typeof(Chem_GetAllTheory) },
                        }
                    },
                    new MenuItemModel
                    {
                        Title = "Hóa chất - Trang thiết bị",
                        SubMenuItems = new List<MenuItemModel>
                        {
                            new MenuItemModel { Title = "Hóa chất", TargetType = typeof(Chem_GetAllTheory) },
                            new MenuItemModel { Title = "Dụng cụ", TargetType = typeof(Chem_GetAllTheory) },
                            new MenuItemModel { Title = "Thiết bị", TargetType = typeof(Chem_GetAllTheory) },
                        }
                    },
                    new MenuItemModel
                    {
                        Title = "Dự trù",
                        SubMenuItems = new List<MenuItemModel>
                        {
                            new MenuItemModel { Title = "Hóa chất", TargetType = typeof(Chem_GetAllTheory) },
                            new MenuItemModel { Title = "Dụng cụ", TargetType = typeof(Chem_GetAllTheory) },
                            new MenuItemModel { Title = "Thiết bị", TargetType = typeof(Chem_GetAllTheory) },
                        }
                    },
                    new MenuItemModel
                    {
                        Title = "Thực hành Hóa học",
                        SubMenuItems = new List<MenuItemModel>
                        {
                            new MenuItemModel { Title = "Lịch thực hành", TargetType = typeof(Chem_GetAllTheory) },
                            new MenuItemModel { Title = "Câu hỏi chuẩn bị", TargetType = typeof(Chem_GetAllTheory) },
                            new MenuItemModel { Title = "Báo cáo thực hành", TargetType = typeof(Chem_GetAllTheory) },
                            new MenuItemModel { Title = "Điểm thực hành", TargetType = typeof(Chem_GetAllTheory) },
                        }
                     },
                     new MenuItemModel
                     {
                        Title = "Virtual Lab",
                        SubMenuItems = new List<MenuItemModel>
                        {
                            new MenuItemModel { Title = "Lý thuyết", TargetType = typeof(Chem_GetAllTheory) },
                            new MenuItemModel { Title = "Thực hành", TargetType = typeof(Chem_GetAllTheory) },
                            new MenuItemModel { Title = "Thực nghiệm", TargetType = typeof(Chem_GetAllTheory) },
                        }
                     },
                     new MenuItemModel
                     {
                        Title = "Nhân sự bộ môn",
                        SubMenuItems = new List<MenuItemModel>
                        {
                            new MenuItemModel { Title = "Ban chủ nhiệm", TargetType = typeof(Chem_GetAllTheory) },
                            new MenuItemModel { Title = "Giảng viên", TargetType = typeof(Chem_GetAllTheory) },
                            new MenuItemModel { Title = "Tổ kỹ thuật", TargetType = typeof(Chem_GetAllTheory) },
                        }
                     },
                     new MenuItemModel
                     {
                        Title = "Nghiên cứu khoa học",
                        SubMenuItems = new List<MenuItemModel>
                        {
                            new MenuItemModel { Title = "Đề tài cấp Cơ sở", TargetType = typeof(Chem_GetAllTheory) },
                            new MenuItemModel { Title = "Đề tài cấp Thành phố, cấp Tỉnh", TargetType = typeof(Chem_GetAllTheory) },
                            new MenuItemModel { Title = "Đề tài cấp Bộ, cấp Quốc gia", TargetType = typeof(Chem_GetAllTheory) },
                        }
                     },
                     new MenuItemModel
                     {
                        Title = "Tiếng anh chuyên ngành",
                        SubMenuItems = new List<MenuItemModel>
                        {
                            new MenuItemModel { Title = "Từ vựng", TargetType = typeof(Chem_GetAllTheory) },
                            new MenuItemModel { Title = "Bài giảng song ngữ", TargetType = typeof(Chem_GetAllTheory) },
                            new MenuItemModel { Title = "Bài tập", TargetType = typeof(Chem_GetAllTheory) },
                        }
                     },
                }
            },
            new MenuItemModel
            {
                Title = "Biology",
                SubMenuItems = new List<MenuItemModel>
                {
                    new MenuItemModel { Title = "Bio Submenu 1", TargetType = typeof(Bio_GetAllTheory) },
                    new MenuItemModel { Title = "Bio Submenu 2", TargetType = typeof(Bio_GetAllTheory) }
                }
            },
            new MenuItemModel
            {
                Title = "Physics",
                SubMenuItems = new List<MenuItemModel>
                {
                    new MenuItemModel { Title = "Phys Submenu 1", TargetType = typeof(Phys_GetAllTheory) },
                    new MenuItemModel { Title = "Phys Submenu 2", TargetType = typeof(Phys_GetAllTheory) }
                }
            }
        };
    }
}

