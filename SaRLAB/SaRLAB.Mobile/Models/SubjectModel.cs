namespace SaRLAB.Mobile.Models;

public class SubjectModel
{
    public string SubjectName { get; set; }
    public string SubjectLogo { get; set; }
    public string SubjectBackground { get; set; }

    public Color SubjectBackgroundColor
    {
        get
        {
            return Color.FromArgb(SubjectBackground);
        }
    }
}
