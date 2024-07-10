using SaRLAB.Mobile.View;
using SaRLAB.Mobile.View.Biochem;
using SaRLAB.Mobile.View.Biology;
using SaRLAB.Mobile.View.Chemistry;
using SaRLAB.Mobile.View.Physics;

namespace SaRLAB.Mobile;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        Routing.RegisterRoute(nameof(SubjectMenuPage), typeof(SubjectMenuPage));

        // chem
        Routing.RegisterRoute(nameof(Chem_GetAllTheory), typeof(Chem_GetAllTheory));
        Routing.RegisterRoute(nameof(Chem_QuizPage), typeof(Chem_QuizPage));

        // bio
        Routing.RegisterRoute(nameof(Bio_GetAllTheory), typeof(Bio_GetAllTheory));
        Routing.RegisterRoute(nameof(Bio_QuizPage), typeof(Bio_QuizPage));

        // phys
        Routing.RegisterRoute(nameof(Phys_GetAllTheory), typeof(Phys_GetAllTheory));
        Routing.RegisterRoute(nameof(Phys_QuizPage), typeof(Phys_QuizPage));

        // biochem
        Routing.RegisterRoute(nameof(BioChem_GetAllTheory), typeof(BioChem_GetAllTheory));
        Routing.RegisterRoute(nameof(BioChem_QuizPage), typeof(BioChem_QuizPage));
    }
}
