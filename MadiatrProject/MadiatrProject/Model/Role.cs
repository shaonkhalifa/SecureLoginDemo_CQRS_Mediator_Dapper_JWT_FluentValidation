using System.ComponentModel.DataAnnotations;

namespace MadiatrProject.Model
{
    public class Role
    {
        [Key]
        public int RoleId { get; set; }
        public string? RoleName { get; set; }
    }
}
