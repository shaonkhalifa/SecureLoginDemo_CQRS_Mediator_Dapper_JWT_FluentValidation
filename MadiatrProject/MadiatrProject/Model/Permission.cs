using System.ComponentModel.DataAnnotations;

namespace MadiatrProject.Model
{
    public class Permission
    {
        [Key]
        public int PermissionId { get; set; }
        public string PermissionName { get; set; }
    }
}
