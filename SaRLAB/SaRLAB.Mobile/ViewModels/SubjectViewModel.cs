namespace SaRLAB.Mobile.ViewModels;

using SaRLAB.Mobile.Models;
using System.Collections.ObjectModel;

public class SubjectViewModel
{
    public ObservableCollection<SubjectModel> Subjects { get; set; }

    public SubjectViewModel()
    {
        Subjects = new ObservableCollection<SubjectModel>
        {
            new SubjectModel { SubjectName = "CHemLAB", SubjectLogo = "logo_chem.png", SubjectBackground = "#ad8200" },
            new SubjectModel { SubjectName = "BiOLAB", SubjectLogo = "logo_bio.png", SubjectBackground = "#0d4291" },
            new SubjectModel { SubjectName = "PhysLAB", SubjectLogo = "logo_phys.png", SubjectBackground = "#075a33" },
            new SubjectModel { SubjectName = "BioChemLAB", SubjectLogo = "logo_biochem.png", SubjectBackground = "#951515" },
            new SubjectModel { SubjectName = "Subject1LAB", SubjectLogo = "logo_book.png", SubjectBackground = "#834d0a" },
            new SubjectModel { SubjectName = "Subject2LAB", SubjectLogo = "logo_book.png", SubjectBackground = "#ad8200" },
            new SubjectModel { SubjectName = "Subject3LAB", SubjectLogo = "logo_book.png", SubjectBackground = "#0d4291" },
            new SubjectModel { SubjectName = "Subject4LAB", SubjectLogo = "logo_book.png", SubjectBackground = "#075a33" },
            new SubjectModel { SubjectName = "Subject5LAB", SubjectLogo = "logo_book.png", SubjectBackground = "#951515" },
            new SubjectModel { SubjectName = "Subject6LAB", SubjectLogo = "logo_book.png", SubjectBackground = "#834d0a" },
        };
    }
}
