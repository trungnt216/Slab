namespace SaRLAB.MobileApp.Models;

public class User
{
    public int ID { get; set; }
    public string? Name { get; set; }
    public string? LoginName { get; set; }
    public string? Password { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public string? CreateBy { get; set; }
    public string? UpdateBy { get; set; }
    public DateTime? CreateTime { get; set; }
    public string? Role { get; set; }
}
