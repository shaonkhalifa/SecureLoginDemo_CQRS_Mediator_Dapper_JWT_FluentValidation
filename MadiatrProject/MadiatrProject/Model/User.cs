using System.ComponentModel.DataAnnotations;

namespace MadiatrProject.Model;

public class User
{
    [Key]
    public int UserId { get; set; }
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string UserName { get; set; } = null!;
    public string Password { get; set; } = null!;
    public string? Token { get; set; }
}
